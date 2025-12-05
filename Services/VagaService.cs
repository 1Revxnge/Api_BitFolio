using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using BitFolio.models.DTOs;
using BitFolio.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services
{
    public class VagaService : IVagaService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        public Func<HttpClient>? HttpClientFactoryOverride { get; set; }

        private static readonly string _templateAtualizacaoVaga = "https://bitfolio-s3.s3.sa-east-1.amazonaws.com/AtualizacaoVagaTemplate.html";
        public VagaService(AppDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }
        private HttpClient CriarHttpClient()
        {
            return HttpClientFactoryOverride?.Invoke() ?? new HttpClient();
        }
        public async Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize)
        {
            var dataAtual = DateTime.UtcNow;

            return await _dbContext.Vagas
                .Where(v => v.Ativo && (v.DataFechamento == null || v.DataFechamento >= dataAtual))
                .OrderByDescending(v => v.DataAbertura)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalVagasAsync()
        {
            var dataAtual = DateTime.UtcNow;

            return await _dbContext.Vagas
                .Where(v => v.Ativo && (v.DataFechamento == null || v.DataFechamento >= dataAtual))
                .CountAsync();
        }
        public async Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid empresaId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            return await _dbContext.Vagas
                .Where(v => v.EmpresaId == empresaId)
                .OrderByDescending(v => v.DataAbertura)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalVagasByEmpresaAsync(Guid empresaId)
        {
            return await _dbContext.Vagas
                .CountAsync(v => v.EmpresaId == empresaId);
        }
        public async Task<Vaga?> GetVagaByIdAsync(Guid id)
        {
            return await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);
        }

        public async Task<Vaga> AddVagaAsync(Vaga vaga)
        {
            if (vaga.DataAbertura.HasValue)
                vaga.DataAbertura = DateTime.SpecifyKind(vaga.DataAbertura.Value, DateTimeKind.Utc);

            if (vaga.DataFechamento.HasValue)
                vaga.DataFechamento = DateTime.SpecifyKind(vaga.DataFechamento.Value, DateTimeKind.Utc);

            _dbContext.Vagas.Add(vaga);
            await _dbContext.SaveChangesAsync();
            return vaga;
        }



        public async Task<Vaga?> UpdateVagaAsync(Vaga vaga)
        {
            var existingVaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == vaga.VagaId);

            if (existingVaga == null)
                return null;

            existingVaga.Titulo = vaga.Titulo;
            existingVaga.Descricao = vaga.Descricao;
            existingVaga.Nivel = vaga.Nivel;
            existingVaga.Modelo = vaga.Modelo;
            existingVaga.Tecnologias = vaga.Tecnologias;
            existingVaga.Area = vaga.Area;
            existingVaga.Requisitos = vaga.Requisitos;
            existingVaga.DataAbertura = vaga.DataAbertura.HasValue
                ? DateTime.SpecifyKind(vaga.DataAbertura.Value, DateTimeKind.Utc)
                : null;
            existingVaga.DataFechamento = vaga.DataFechamento.HasValue
                ? DateTime.SpecifyKind(vaga.DataFechamento.Value, DateTimeKind.Utc)
                : null;
            existingVaga.Salario = vaga.Salario;

            await _dbContext.SaveChangesAsync();
            return existingVaga;
        }

        public async Task<bool> DeleteVagaAsync(Guid id)
        {
            // Busca a vaga
            var vaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);

            if (vaga == null)
                return false;

            var historicos = _dbContext.HistoricoCandidaturas
                .Where(h => h.VagaId == id);
            _dbContext.HistoricoCandidaturas.RemoveRange(historicos);

            var candidatoVagas = _dbContext.CandidatoVagas
                .Where(cv => cv.VagaId == id);
            _dbContext.CandidatoVagas.RemoveRange(candidatoVagas);

         
            _dbContext.Vagas.Remove(vaga);

          
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleFavoritoAsync(Guid candidatoId, Guid vagaId)
        {
            var favorito = await _dbContext.VagasFavoritas
                .FirstOrDefaultAsync(f => f.CandidatoId == candidatoId && f.VagaId == vagaId);

            bool favoritado;

            if (favorito != null)
            {
                // Se já existe, desfavorita
                _dbContext.VagasFavoritas.Remove(favorito);
                favoritado = false;
            }
            else
            {
                // Se não existe, favorita
                var novoFavorito = new VagaFavorita
                {
                    Id = Guid.NewGuid(),
                    CandidatoId = candidatoId,
                    VagaId = vagaId
                };
                await _dbContext.VagasFavoritas.AddAsync(novoFavorito);
                favoritado = true;
            }

            // Agora salva as alterações sempre
            await _dbContext.SaveChangesAsync();

            return favoritado;
        }

        public async Task<IEnumerable<Vaga>> GetFavoritosByCandidatoAsync(Guid candidatoId)
        {
            return await _dbContext.VagasFavoritas
                .Where(f => f.CandidatoId == candidatoId)
                .Select(f => f.Vaga!)
                .ToListAsync();
        }

        public async Task<(IEnumerable<VagaDTO> Vagas, int TotalCount)> BuscarPorFiltros(FiltroVagaDTO filtro, Guid candidatoId)
        {
            var query = _dbContext.Vagas
                .Include(v => v.Empresa)
                    .ThenInclude(e => e.Endereco)
                .Where(v => v.Ativo)
                .Where(v => !v.DataFechamento.HasValue || v.DataFechamento >= DateTime.UtcNow)
                .AsQueryable();

            // Palavras-chave
            if (!string.IsNullOrWhiteSpace(filtro.PalavrasChave))
            {
                var termos = filtro.PalavrasChave.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var termo in termos)
                {
                    var temp = termo.ToLower();
                    query = query.Where(v =>
                        (v.Titulo ?? "").ToLower().Contains(temp) ||
                        (v.Descricao ?? "").ToLower().Contains(temp));
                }
            }

            if (!string.IsNullOrWhiteSpace(filtro.Linguagens))
            {
                var linguagensList = filtro.Linguagens
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().ToLower())
                    .ToList();

                query = query.Where(v =>
                    linguagensList.All(lang =>
                        (v.Tecnologias ?? "").ToLower().Contains(lang)
                    )
                );
            }



            if (!string.IsNullOrWhiteSpace(filtro.Experiencia))
                query = query.Where(v => v.Nivel != null &&
                                         v.Nivel.ToLower() == filtro.Experiencia.ToLower());

            if (!string.IsNullOrWhiteSpace(filtro.Modelo))
                query = query.Where(v => v.Modelo != null &&
                                         v.Modelo.ToLower() == filtro.Modelo.ToLower());
            if (!string.IsNullOrWhiteSpace(filtro.Area))
                query = query.Where(v => v.Area != null &&
                                         v.Area.ToLower() == filtro.Area.ToLower());

            // Obter coordenadas do candidato
            var candidato = await _dbContext.Candidatos
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.CandidatoId == candidatoId);

            double? usuarioLat = candidato?.Endereco?.Latitude;
            double? usuarioLon = candidato?.Endereco?.Longitude;

            var vagasComDistancia = (await query
                .Select(v => new VagaDTO
                {
                    VagaId = v.VagaId,
                    Titulo = v.Titulo,
                    Nivel = v.Nivel,
                    Escolaridade = v.Escolaridade,
                    Modelo = v.Modelo,
                    DataAbertura = v.DataAbertura,
                    DataFechamento = v.DataFechamento,
                    Requisitos = v.Requisitos,
                    Descricao = v.Descricao,
                    Ativo = v.Ativo,
                    Tecnologias = v.Tecnologias,
                    Area = v.Area,
                    EmpresaId = v.EmpresaId,
                    EmpresaNome = v.Empresa != null ? v.Empresa.Nome : null,
                    Endereco = v.Empresa.Endereco != null
                        ? $"{v.Empresa.Endereco.Rua}, {v.Empresa.Endereco.Numero}" +
                          $"{(string.IsNullOrEmpty(v.Empresa.Endereco.Complemento) ? "" : $" - {v.Empresa.Endereco.Complemento}")}, " +
                          $"{v.Empresa.Endereco.Bairro}, {v.Empresa.Endereco.Cidade} - {v.Empresa.Endereco.Estado}"
                        : null,
                    Distancia = (usuarioLat.HasValue && usuarioLon.HasValue &&
                                 v.Empresa.Endereco.Latitude.HasValue && v.Empresa.Endereco.Longitude.HasValue)
                        ? 6371 * 2 * Math.Asin(Math.Sqrt(
                            Math.Pow(Math.Sin((v.Empresa.Endereco.Latitude.Value - usuarioLat.Value) * Math.PI / 180 / 2), 2) +
                            Math.Cos(usuarioLat.Value * Math.PI / 180) *
                            Math.Cos(v.Empresa.Endereco.Latitude.Value * Math.PI / 180) *
                            Math.Pow(Math.Sin((v.Empresa.Endereco.Longitude.Value - usuarioLon.Value) * Math.PI / 180 / 2), 2)
                          ))
                        : (double?)null
                })
                .ToListAsync())
                // Filtrar pela proximidade
                .Where(v => !filtro.Proximidade.HasValue || (v.Distancia.HasValue && v.Distancia.Value <= filtro.Proximidade.Value))
                .OrderBy(v => v.Distancia ?? double.MaxValue)
                .ToList();

            int totalCount = vagasComDistancia.Count;

            // Paginação
            if (filtro.Take > 0)
            {
                int skip = (filtro.Page - 1) * filtro.Take;
                vagasComDistancia = vagasComDistancia.Skip(skip).Take(filtro.Take).ToList();
            }

            return (vagasComDistancia, totalCount);
        }



        public ResultadoCandidaturaDTO Candidatar(Guid candidatoId, Guid vagaId)
        {
            var candidato = _dbContext.Candidatos.Include(c => c.Curriculo).FirstOrDefault(c => c.CandidatoId == candidatoId);
            if (candidato == null)
            {
                return new ResultadoCandidaturaDTO
                {
                    Sucesso = false,
                    Mensagem = "Candidato não encontrado.",
                    Codigo = 0
                };
            }
            // Verifica se o candidato tem currículo
            if (candidato.Curriculo == null)
            {
                return new ResultadoCandidaturaDTO
                {
                    Sucesso = false,
                    Mensagem = "É necessário possuir um currículo cadastrado antes de se candidatar a uma vaga.",
                    Codigo = 1
                };
            }
            // Verifica se a vaga existe
            var vaga = _dbContext.Vagas.FirstOrDefault(v => v.VagaId == vagaId);
            if (vaga == null)
            {
                return new ResultadoCandidaturaDTO
                {
                    Sucesso = false,
                    Mensagem = "Vaga não encontrada.",
                    Codigo = 2
                };
            }
            // Verifica se o candidato já está vinculado a essa vaga
            bool jaCandidatado = _dbContext.CandidatoVagas
                .Any(cv => cv.CandidatoId == candidatoId && cv.VagaId == vagaId);
            if (jaCandidatado)
            {
                return new ResultadoCandidaturaDTO
                {
                    Sucesso = false,
                    Mensagem = "Você já se candidatou a esta vaga.",
                    Codigo = 3
                };
            }
            // Cria vínculo entre candidato e vaga
            var candidatoVaga = new CandidatoVaga
            {
                Id = Guid.NewGuid(),
                CandidatoId = candidatoId,
                VagaId = vagaId
            };
            _dbContext.CandidatoVagas.Add(candidatoVaga);
            // Cria histórico de candidatura
            var historico = new HistoricoCandidatura
            {
                HistoricoId = Guid.NewGuid(),
                CandidatoId = candidatoId,
                VagaId = vagaId,
                Status = StatusVaga.CVRecebido,
                DtCandidatura = DateTime.UtcNow,
                DtAtualizacao = DateTime.UtcNow
            };

            _dbContext.HistoricoCandidaturas.Add(historico);
            _dbContext.SaveChanges();
            var empresa = _dbContext.Empresas
    .FirstOrDefault(e => e.EmpresaId == vaga.EmpresaId);

            var log = new LogCandidato
            {
                LogId = Guid.NewGuid(),
                CandidatoId = candidatoId,
                Acao = $"Candidatura enviada para {vaga.Titulo} na {empresa?.Nome ?? "Empresa Desconhecida"}",
                DtAcao = DateTime.UtcNow
            };

            _dbContext.LogCandidatos.Add(log);
            _dbContext.SaveChangesAsync();

            return new ResultadoCandidaturaDTO
            {
                Sucesso = true,
                HistoricoId = historico.HistoricoId
            };
        }
        public async Task<object> AtualizarStatusAsync(AtualizarStatusRequest request)
        {
            var candidatura = await _dbContext.HistoricoCandidaturas
                .FirstOrDefaultAsync(c =>
                    c.CandidatoId == request.CandidatoId &&
                    c.VagaId == request.VagaId);

            if (candidatura == null)
                throw new KeyNotFoundException("Candidatura não encontrada.");

            await _dbContext.Entry(candidatura).Reference(c => c.Candidato).LoadAsync();
            await _dbContext.Entry(candidatura).Reference(c => c.Vaga).LoadAsync();
            if (candidatura.Vaga != null)
                await _dbContext.Entry(candidatura.Vaga).Reference(v => v.Empresa).LoadAsync();

            if (candidatura.Status == request.NovoStatus)
                throw new InvalidOperationException("O status informado é igual ao status atual.");

            bool statusFinal = candidatura.Status == StatusVaga.CVSelecionado ||
                   candidatura.Status == StatusVaga.CVNaoSelecionado;

            if (statusFinal && request.NovoStatus < candidatura.Status)
                throw new InvalidOperationException("Não é permitido retornar o status após Selecionado ou Não Selecionado.");


            var statusAnterior = candidatura.Status;

            candidatura.Status = request.NovoStatus;
            candidatura.DtAtualizacao = DateTime.UtcNow;

            _dbContext.HistoricoCandidaturas.Update(candidatura);
            await _dbContext.SaveChangesAsync();

            try
            {
                using var httpClient = CriarHttpClient();
                var templateHtml = await httpClient.GetStringAsync(_templateAtualizacaoVaga);

                templateHtml = templateHtml
                    .Replace("{{NOME_USUARIO}}", candidatura.Candidato?.Nome ?? "Candidato")
                    .Replace("{{TITULO_VAGA}}", candidatura.Vaga?.Titulo ?? "Vaga")
                    .Replace("{{NOME_EMPRESA}}", candidatura.Vaga?.Empresa?.Nome ?? "Empresa")
                    .Replace("{{STATUS_ANTERIOR}}", GetStatusLabel(statusAnterior))
                    .Replace("{{NOVO_STATUS}}", GetStatusLabel(candidatura.Status));

                await _emailService.EnviarEmailAsync(
                    candidatura.Candidato?.Email,
                    $"Atualização no status da vaga: {candidatura.Vaga?.Titulo}",
                    templateHtml
                );
            }
            catch (Exception ex)
            {
                return (ex, "Erro ao enviar e-mail de atualização de status");
            }

            var logCandidato = new LogCandidato
            {
                LogId = Guid.NewGuid(),
                CandidatoId = candidatura.CandidatoId,
                Acao = $"Status alterado de {GetStatusLabel(statusAnterior)} para {GetStatusLabel(candidatura.Status)} na vaga {candidatura.Vaga?.Titulo}",
                DtAcao = DateTime.UtcNow
            };

            _dbContext.LogCandidatos.Add(logCandidato);
            await _dbContext.SaveChangesAsync();

            var recrutador = await _dbContext.Recrutadores
            .FirstOrDefaultAsync(r => r.EmpresaId == candidatura.Vaga.EmpresaId);

            if (recrutador != null)
            {
                var logRecrutador = new LogRecrutador
                {
                    LogId = Guid.NewGuid(),
                    RecrutadorId = recrutador.RecrutadorId,
                    Acao = $"O candidato {candidatura.Candidato?.Nome} teve o status alterado de {GetStatusLabel(statusAnterior)} para {GetStatusLabel(candidatura.Status)} na vaga {candidatura.Vaga?.Titulo}",
                    DtAcao = DateTime.UtcNow
                };

                _dbContext.LogRecrutadores.Add(logRecrutador);
                await _dbContext.SaveChangesAsync();
            }


            return new
            {
                mensagem = "Status atualizado com sucesso.",
                novoStatus = candidatura.Status.ToString(),
                candidaturaId = candidatura.HistoricoId
            };
        }

        public async Task<IEnumerable<HistoricoCandidatura>> GetHistoricoAsync(Guid candidatoId)
        {
            return await _dbContext.HistoricoCandidaturas
                .Where(h => h.CandidatoId == candidatoId)
                .Include(h => h.Vaga)
                    .ThenInclude(v => v.Empresa)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<IEnumerable<CandidatoVaga>> GetCandidatosDaVagaAsync(Guid vagaId, int? status, string? search)
        {
            var query = _dbContext.CandidatoVagas
                .Where(cv => cv.VagaId == vagaId)
                .Include(cv => cv.Candidato)
                    .ThenInclude(c => c.Curriculo)
                .Include(cv => cv.Candidato)
                    .ThenInclude(c => c.Historicos)
                .AsQueryable();

            if (status.HasValue)
            {
                var enumStatus = (StatusVaga)status.Value;
                query = query.Where(cv =>
                    cv.Candidato.Historicos
                        .Where(h => h.VagaId == vagaId)
                        .OrderByDescending(h => h.DtAtualizacao)
                        .FirstOrDefault()!.Status == enumStatus);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower().Trim();
                query = query.Where(cv =>
                    cv.Candidato.Nome.ToLower().Contains(searchLower) ||
                    cv.Candidato.Email.ToLower().Contains(searchLower));
            }

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CandidatoStatusCountDto> GetCandidatosCountsAsync(Guid vagaId)
        {
            var historicos = await _dbContext.HistoricoCandidaturas
                .Where(h => h.VagaId == vagaId)
                .GroupBy(h => h.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var dto = new CandidatoStatusCountDto
            {
                Total = historicos.Sum(h => h.Count),
                EmAnalise = historicos.FirstOrDefault(h => h.Status == StatusVaga.CVRecebido)?.Count ?? 0,
                Revisado = historicos.FirstOrDefault(h => h.Status == StatusVaga.CVSelecionado)?.Count ?? 0,
                Entrevista = historicos.FirstOrDefault(h => h.Status == StatusVaga.CVPreSelecionado)?.Count ?? 0,
                Aprovados = historicos.FirstOrDefault(h => h.Status == StatusVaga.CVSelecionado)?.Count ?? 0,
                Rejeitados = historicos.FirstOrDefault(h => h.Status == StatusVaga.CVNaoSelecionado)?.Count ?? 0
            };

            return dto;
        }
        public static string GetStatusLabel(StatusVaga status)
        {
            return status switch
            {
                StatusVaga.CVRecebido => "Em Análise",
                StatusVaga.CVRevisado => "Curriculo Revisado",
                StatusVaga.CVPreSelecionado => "Pré-Selecionado",
                StatusVaga.CVSelecionado => "Selecionado",
                StatusVaga.CVNaoSelecionado => "Não Selecionado",
                _ => "Desconhecido"
            };
        }
    }

}