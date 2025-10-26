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

        private static readonly string _templateAtualizacaoVaga = "https://bitfolio-s3.s3.sa-east-1.amazonaws.com/AtualizacaoVagaTemplate.html";
        public VagaService(AppDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize)
        {
            return await _dbContext.Vagas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalVagasAsync()
        {
            return await _dbContext.Vagas.CountAsync();
        }
        public async Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid Empresas)
        {
            return await _dbContext.Vagas
                .Where(v => v.EmpresaId == Empresas)
                .ToListAsync();
        }

        public async Task<Vaga?> GetVagaByIdAsync(Guid id)
        {
            return await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);
        }

        public async Task<Vaga> AddVagaAsync(Vaga vaga)
        {
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
            existingVaga.Requisitos = vaga.Requisitos;
            existingVaga.DataAbertura = vaga.DataAbertura;
            existingVaga.DataFechamento = vaga.DataFechamento;


            await _dbContext.SaveChangesAsync();
            return existingVaga; 
        }

        public async Task<bool> DeleteVagaAsync(Guid id)
        {
            var vaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);

            if (vaga == null)
                return false;

            _dbContext.Vagas.Remove(vaga);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleFavoritoAsync(Guid candidatoId, Guid vagaId)
        {
            var favorito = await _dbContext.VagasFavoritas
                .FirstOrDefaultAsync(f => f.CandidatoId == candidatoId && f.VagaId == vagaId);

            if (favorito != null)
            {
                // Se já existe, desfavorita
                _dbContext.VagasFavoritas.Remove(favorito);
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
            }

            await _dbContext.SaveChangesAsync();
            return true;
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
                .Where(v => v.Ativo) // apenas vagas ativas
                .Where(v => !v.DataFechamento.HasValue || v.DataFechamento >= DateTime.UtcNow) // ainda não fechadas
                .AsQueryable();

            // 🔎 Palavras-chave
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

            // 💻 Linguagens
            if (filtro.Linguagens != null && filtro.Linguagens.Any())
            {
                query = query.Where(v => filtro.Linguagens.Any(lang =>
                    (v.Tecnologias ?? "").ToLower().Contains(lang.ToLower())));
            }

            // 🧠 Nível de experiência
            if (!string.IsNullOrWhiteSpace(filtro.Experiencia))
                query = query.Where(v => v.Nivel != null &&
                                         v.Nivel.ToLower() == filtro.Experiencia.ToLower());

            // 🏢 Área
            if (!string.IsNullOrWhiteSpace(filtro.Area))
                query = query.Where(v => v.Area != null &&
                                         v.Area.ToLower() == filtro.Area.ToLower());

            // 📍 Obter coordenadas do candidato
            var candidato = await _dbContext.Candidatos
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.CandidatoId == candidatoId);

            double? usuarioLat = candidato?.Endereco?.Latitude;
            double? usuarioLon = candidato?.Endereco?.Longitude;

            // 🔹 Projeção para DTO com cálculo de distância
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
                // 🔹 Filtrar pela proximidade
                .Where(v => !filtro.Proximidade.HasValue || (v.Distancia.HasValue && v.Distancia.Value <= filtro.Proximidade.Value))
                .OrderBy(v => v.Distancia ?? double.MaxValue)
                .ToList();

            int totalCount = vagasComDistancia.Count;

            // 🔹 Paginação
            if (filtro.Take > 0)
            {
                int skip = (filtro.Page - 1) * filtro.Take;
                vagasComDistancia = vagasComDistancia.Skip(skip).Take(filtro.Take).ToList();
            }

            return (vagasComDistancia, totalCount);
        }



        public ResultadoCandidaturaDTO Candidatar(Guid candidatoId, Guid vagaId)
        {
            var candidato = _dbContext.Candidatos
                .Include(c => c.Curriculo)
                .FirstOrDefault(c => c.CandidatoId == candidatoId);

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

            return new ResultadoCandidaturaDTO
            {
                Sucesso = true,
                HistoricoId = historico.HistoricoId
            };
        }
        public async Task<object> AtualizarStatusAsync(AtualizarStatusRequest request)
        {
            var candidatura = await _dbContext.HistoricoCandidaturas
            .Include(c => c.Candidato)
            .Include(c => c.Vaga)
            .ThenInclude(v => v.Empresa)
            .FirstOrDefaultAsync(c =>
                c.CandidatoId == request.CandidatoId &&
                c.VagaId == request.VagaId);

            if (candidatura == null)
                throw new KeyNotFoundException("Candidatura não encontrada.");

            //não permitir alterações em status finalizados
            if (candidatura.Status == StatusVaga.CVSelecionado || candidatura.Status == StatusVaga.CVNaoSelecionado)
                throw new InvalidOperationException("Não é possível alterar o status de uma candidatura finalizada.");

            //evitar atualizações redundantes
            if (candidatura.Status == request.NovoStatus)
                throw new InvalidOperationException("O status informado é igual ao status atual.");
                
            var statusAnterior = candidatura.Status;

            //Atualiza o status e data
            candidatura.Status = request.NovoStatus;
            candidatura.DtAtualizacao = DateTime.UtcNow;

            _dbContext.HistoricoCandidaturas.Update(candidatura);
            await _dbContext.SaveChangesAsync();

            try
            {

                using var httpClient = new HttpClient();
                var templateHtml = await httpClient.GetStringAsync(_templateAtualizacaoVaga);

                // substitui placeholders
                templateHtml = templateHtml
                    .Replace("{{NOME_USUARIO}}", candidatura.Candidato?.Nome ?? "Candidato")
                    .Replace("{{TITULO_VAGA}}", candidatura.Vaga?.Titulo ?? "Vaga")
                    .Replace("{{NOME_EMPRESA}}", candidatura.Vaga?.Empresa?.Nome ?? "Empresa")
                    .Replace("{{STATUS_ANTERIOR}}", statusAnterior.ToString())
                    .Replace("{{NOVO_STATUS}}", candidatura.Status.ToString());

                // envia e-mail
                await _emailService.EnviarEmailAsync(
                    candidatura.Candidato?.Email,
                    $"Atualização no status da vaga: {candidatura.Vaga?.Titulo}",
                    templateHtml
                );
            }
            catch (Exception ex)
            {
               return(ex, "Erro ao enviar e-mail de atualização de status");
            }
            return new
            {
                mensagem = "Status atualizado com sucesso.",
                novoStatus = candidatura.Status.ToString(),
                candidaturaId = candidatura.HistoricoId
            };
        }
    }
}

