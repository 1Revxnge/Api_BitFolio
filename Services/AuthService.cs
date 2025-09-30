using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiJobfy.Services.IService;

namespace ApiJobfy.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(AppDbContext dbContext, IConfiguration configuration, IEmailService emailservice)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailservice;
        }

        public async Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto)
        {
            // Hash password
            string senhaHash = HashPassword(dto.Senha);
            DateTime agora = DateTime.Now;

            var candidato = new Candidato
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = senhaHash,
                DataNascimento = dto.DataNascimento,
                Telefone = dto.Telefone,
                Ativo = false
            };
            _dbContext.Candidatos.Add(candidato);
            await _dbContext.SaveChangesAsync();

            // Gerar token de validação
            var token = Guid.NewGuid().ToString("N");
            var tokenEmail = new TokenTemporario
            {
                Tipo = TipoToken.ValidacaoEmail,
                Email = candidato.Email,
                Codigo = token,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddHours(24),
                Utilizado = false
            };
            _dbContext.TokenTemporario.Add(tokenEmail);
            await _dbContext.SaveChangesAsync();

            // Montar link e corpo do e-mail
            var link = $"http://localhost:5272/api/auth/confirmar-email?token={token}";
            var corpoEmail = $@"
        <h2>Confirme seu cadastro no BitFolio</h2>
        <p>Olá {candidato.Nome}, clique no botão abaixo para ativar sua conta:</p>
        <a href='{link}' 
           style='display:inline-block;padding:10px 20px;
                  color:white;background:#28a745;text-decoration:none;
                  border-radius:5px;'>Validar Cadastramento</a>
        <p>Se você não solicitou este cadastro, ignore este e-mail.</p>";

            await _emailService.EnviarEmailAsync(
                candidato.Email,
                "Confirmação de Cadastro - BitFolio",
                corpoEmail
            );

            return candidato;
        }

        public async Task<Recrutador> RegisterFuncionarioAsync(RegisterFuncionarioDto dto)
        {
            string senhaHash = HashPassword(dto.Senha);
            DateTime agora = DateTime.Now;

            var funcionario = new Recrutador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = senhaHash,
                Telefone = dto.Telefone,
                EmpresaId = dto.EmpresaId,
                Ativo = false
            };

            _dbContext.Recrutadores.Add(funcionario);
            await _dbContext.SaveChangesAsync();

            // Gerar token de validação
            var token = Guid.NewGuid().ToString("N");
            var tokenEmail = new TokenTemporario
            {
                Tipo = TipoToken.ValidacaoEmail,
                Email = funcionario.Email,
                Codigo = token,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddHours(24),
                Utilizado = false
            };
            _dbContext.TokenTemporario.Add(tokenEmail);
            await _dbContext.SaveChangesAsync();

            // Montar link e corpo do e-mail
            var link = $"http://localhost:5272/api/auth/confirmar-email?token={token}";
            var corpoEmail = $@"
        <h2>Confirme seu cadastro no BitFolio</h2>
        <p>Olá {funcionario.Nome}, clique no botão abaixo para ativar sua conta:</p>
        <a href='{link}' 
           style='display:inline-block;padding:10px 20px;
                  color:white;background:#28a745;text-decoration:none;
                  border-radius:5px;'>Validar Cadastramento</a>
        <p>Se você não solicitou este cadastro, ignore este e-mail.</p>";

            await _emailService.EnviarEmailAsync(
                funcionario.Email,
                "Confirmação de Cadastro - BitFolio",
                corpoEmail
            );

            return funcionario;
        }

        public async Task<Administrador> RegisterAdministradorAsync(RegisterAdminDto dto)
        {
            string senhaHash = HashPassword(dto.Senha);
            DateTime agora = DateTime.Now;

            var administrador = new Administrador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = senhaHash,
                Telefone = dto.Telefone,
                Ativo = false
            };

            _dbContext.Administradores.Add(administrador);
            await _dbContext.SaveChangesAsync();

            // Gerar token de validação
            var token = Guid.NewGuid().ToString("N");
            var tokenEmail = new TokenTemporario
            {
                Tipo = TipoToken.ValidacaoEmail,
                Email = administrador.Email,
                Codigo = token,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddHours(24),
                Utilizado = false
            };
            _dbContext.TokenTemporario.Add(tokenEmail);
            await _dbContext.SaveChangesAsync();

            // Montar link e corpo do e-mail
            var link = $"http://localhost:5272/api/auth/confirmar-email?token={token}";
            var corpoEmail = $@"
        <h2>Confirme seu cadastro no BitFolio</h2>
        <p>Olá {administrador.Nome}, clique no botão abaixo para ativar sua conta:</p>
        <a href='{link}' 
           style='display:inline-block;padding:10px 20px;
                  color:white;background:#28a745;text-decoration:none;
                  border-radius:5px;'>Validar Cadastramento</a>
        <p>Se você não solicitou este cadastro, ignore este e-mail.</p>";

            await _emailService.EnviarEmailAsync(
                administrador.Email,
                "Confirmação de Cadastro - BitFolio",
                corpoEmail
            );

            return administrador;
        }


        public async Task<LoginResult> LoginAsync(string email, string senha, string tipo)
        {
            email = email.ToLower().Trim();
            senha = senha.Trim();

            switch (tipo.ToLower())
            {
                case "candidato":
                    var candidato = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email == email && c.Ativo == true);
                    if (candidato == null)
                        throw new InvalidOperationException("Candidato não encontrado.");

                    if (await EstaBloqueadoPorEmailCandidato(email))
                        throw new InvalidOperationException("Este candidato está temporariamente bloqueado por várias tentativas falhas.");

                    bool senhaValidaCandidato = VerifyPassword(senha, candidato.Senha);
                    await RegistrarLogCandidato(candidato.CandidatoId, senhaValidaCandidato);

                    if (!senhaValidaCandidato)
                    {
                        int restantes = await TentativasRestantesCandidato(candidato.CandidatoId);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }
                    if (candidato.UltimoAcesso == null || candidato.UltimoAcesso <= DateTime.UtcNow.AddDays(-7))
                    {
                        await GerarToken2FAAsync(email);
                        return new LoginResult{DoisFatoresNecessario = true};
                    }
                    // Limpa as tentativas de login e gera o token JWT
                    await LimparTentativasFalhasCandidato(candidato.CandidatoId);
                    candidato.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    await _dbContext.SaveChangesAsync();
                    return new LoginResult {Token = GenerateJwtToken(candidato),DoisFatoresNecessario = false};

                case "administrador":
                    var administrador = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.Email == email && a.Ativo == true);
                    if (administrador == null)
                        throw new InvalidOperationException("Administrador não encontrado.");

                    if (await EstaBloqueadoPorEmailAdministrador(email))
                        throw new InvalidOperationException("Administrador bloqueado por muitas tentativas falhas.");

                    bool senhaValidaAdmin = VerifyPassword(senha, administrador.Senha);
                    await RegistrarLogAdministrador(administrador.AdminId, senhaValidaAdmin);

                    if (!senhaValidaAdmin)
                    {
                        int restantes = await TentativasRestantesAdministrador(administrador.AdminId);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }

                    if (administrador.UltimoAcesso == null || administrador.UltimoAcesso <= DateTime.UtcNow.AddDays(-7))
                    {
                        await GerarToken2FAAsync(email);
                        return new LoginResult { DoisFatoresNecessario = true };
                    }
                    // Limpa as tentativas de login e gera o token JWT
                    await LimparTentativasFalhasAdministrador(administrador.AdminId);
                    administrador.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    await _dbContext.SaveChangesAsync();
                    return new LoginResult { Token = GenerateJwtToken(administrador), DoisFatoresNecessario = false };

                case "funcionario":
                    var funcionario = await _dbContext.Recrutadores.FirstOrDefaultAsync(f => f.Email == email && f.Ativo == true);
                    if (funcionario == null)
                        throw new InvalidOperationException("Funcionário não encontrado.");

                    if (await EstaBloqueadoPorEmailFuncionario(email))
                        throw new InvalidOperationException("Funcionário bloqueado temporariamente.");

                    bool senhaValidaFunc = VerifyPassword(senha, funcionario.Senha);
                    await RegistrarLogFuncionario(funcionario.RecrutadorId, senhaValidaFunc);

                    if (!senhaValidaFunc)
                    {
                        int restantes = await TentativasRestantesFuncionario(funcionario.RecrutadorId);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }

                    if (funcionario.UltimoAcesso == null || funcionario.UltimoAcesso <= DateTime.UtcNow.AddDays(-7))
                    {
                        await GerarToken2FAAsync(email);
                        return new LoginResult { DoisFatoresNecessario = true };
                    }
                    // Limpa as tentativas de login e gera o token JWT
                    await LimparTentativasFalhasFuncionario(funcionario.RecrutadorId);
                    funcionario.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                    await _dbContext.SaveChangesAsync();
                    return new LoginResult { Token = GenerateJwtToken(funcionario), DoisFatoresNecessario = false };

                default:
                    throw new InvalidOperationException("Tipo de usuário inválido.");
            }
        }

        private async Task RegistrarLogCandidato(Guid candidatoId, bool sucesso)
        {
            if (sucesso)
            {
                var logsSucesso = await _dbContext.LogCandidatos
                    .Where(l => l.CandidatoId == candidatoId && l.Acao == "Login bem-sucedido")
                    .ToListAsync();

                if (logsSucesso.Any())
                    _dbContext.LogCandidatos.RemoveRange(logsSucesso);

                _dbContext.LogCandidatos.Add(new LogCandidato
                {
                    CandidatoId = candidatoId,
                    Acao = "Login bem-sucedido",
                    DtAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogCandidatos.Add(new LogCandidato
                {
                    CandidatoId = candidatoId,
                    Acao = "Login falhou",
                    DtAcao = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task RegistrarLogAdministrador(Guid adminId, bool sucesso)
        {
            if (sucesso)
            {
                var logsSucesso = await _dbContext.LogAdministradores
                    .Where(l => l.AdminId == adminId && l.Acao == "Login bem-sucedido")
                    .ToListAsync();

                if (logsSucesso.Any())
                    _dbContext.LogAdministradores.RemoveRange(logsSucesso);

                _dbContext.LogAdministradores.Add(new LogAdministrador
                {
                    AdminId = adminId,
                    Acao = "Login bem-sucedido",
                    DtAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogAdministradores.Add(new LogAdministrador
                {
                    AdminId = adminId,
                    Acao = "Login falhou",
                    DtAcao = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
        }


        private async Task RegistrarLogFuncionario(Guid funcionarioId, bool sucesso)
        {
            if (sucesso)
            {
                var logsSucesso = await _dbContext.LogRecrutadores
                    .Where(l => l.RecrutadorId == funcionarioId && l.Acao == "Login bem-sucedido")
                    .ToListAsync();

                if (logsSucesso.Any())
                    _dbContext.LogRecrutadores.RemoveRange(logsSucesso);

                _dbContext.LogRecrutadores.Add(new LogRecrutador
                {
                    RecrutadorId = funcionarioId,
                    Acao = "Login bem-sucedido",
                    DtAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogRecrutadores.Add(new LogRecrutador
                {
                    RecrutadorId = funcionarioId,
                    Acao = "Login falhou",
                    DtAcao = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> EstaBloqueadoPorEmailCandidato(string email)
        {
            var candidato = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (candidato == null)
                return false;

            var limite = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogCandidatos
                .Where(l => l.CandidatoId == candidato.CandidatoId && l.DtAcao >= limite && l.Acao == "Login falhou")
                .CountAsync();

            return falhas >= 5;
        }

        private async Task<bool> EstaBloqueadoPorEmailAdministrador(string email)
        {
            var admin = await _dbContext.Administradores.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (admin == null)
                return false;

            var limite = DateTime.UtcNow.AddMinutes(-15);

            var logs = await _dbContext.LogAdministradores
                .Where(l => l.AdminId == admin.AdminId && l.DtAcao >= limite && l.Acao.Contains("Login falhou"))
                .OrderByDescending(l => l.DtAcao)
                .Take(5)
                .ToListAsync();

            return logs.Count >= 5;
        }

        private async Task<bool> EstaBloqueadoPorEmailFuncionario(string email)
        {
            var func = await _dbContext.Recrutadores.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (func == null)
                return false;
            var limite = DateTime.UtcNow.AddMinutes(-15);

            var logs = await _dbContext.LogRecrutadores
                .Where(l => l.RecrutadorId == func.RecrutadorId && l.DtAcao >= limite && l.Acao.Contains("Login falhou"))
                .OrderByDescending(l => l.DtAcao)
                .Take(5)
                .ToListAsync();

            return logs.Count >= 5;
        }
        private async Task<int> TentativasRestantesCandidato(Guid candidatoId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogCandidatos
                .CountAsync(l => l.CandidatoId == candidatoId && l.DtAcao >= desde && l.Acao == "Login falhou");

            return Math.Max(0, 5 - falhas);
        }

        private async Task<int> TentativasRestantesAdministrador(Guid adminId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogAdministradores
                .CountAsync(l => l.AdminId == adminId && l.DtAcao >= desde && l.Acao.Contains("Login falhou"));

            return Math.Max(0, 5 - falhas);
        }

        private async Task<int> TentativasRestantesFuncionario(Guid funcionarioId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogRecrutadores
                .CountAsync(l => l.RecrutadorId == funcionarioId && l.DtAcao >= desde && l.Acao.Contains("Login falhou"));

            return Math.Max(0, 5 - falhas);
        }

        private async Task LimparTentativasFalhasCandidato(Guid candidatoId)
        {
            var logsFalhos = _dbContext.LogCandidatos
                .Where(l => l.CandidatoId == candidatoId && l.Acao == "Login falhou");

            _dbContext.LogCandidatos.RemoveRange(logsFalhos);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LimparTentativasFalhasAdministrador(Guid adminId)
        {
            var logsFalhos = _dbContext.LogAdministradores
                .Where(l => l.AdminId == adminId && l.Acao == "Login falhou");

            _dbContext.LogAdministradores.RemoveRange(logsFalhos);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LimparTentativasFalhasFuncionario(Guid funcionarioId)
        {
            var logsFalhos = _dbContext.LogRecrutadores
                .Where(l => l.RecrutadorId == funcionarioId && l.Acao == "Login falhou");

            _dbContext.LogRecrutadores.RemoveRange(logsFalhos);
            await _dbContext.SaveChangesAsync();
        }
        // Hash password with salt using PBKDF2
        private string HashPassword(string password)
        {
            password = password.Trim(); // evitar espaços acidentais

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            byte[] result = new byte[49];
            result[0] = 0x00;
            Buffer.BlockCopy(salt, 0, result, 1, 16);
            Buffer.BlockCopy(hash, 0, result, 17, 32);

            return Convert.ToBase64String(result);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            password = password.Trim();
            try
            {
                //Verifica os hashs entre a senha do banco de dados e a senha inserida no site
                var decoded = Convert.FromBase64String(storedHash);

                if (decoded.Length != 49 || decoded[0] != 0x00)
                    return false;

                byte[] salt = new byte[16];
                Buffer.BlockCopy(decoded, 1, salt, 0, 16);

                byte[] hash = new byte[32];
                Buffer.BlockCopy(decoded, 17, hash, 0, 32);

                byte[] testHash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 32);

   

                bool matches = CryptographicOperations.FixedTimeEquals(hash, testHash);

                if (!matches)
                {
                    Console.WriteLine("Hashes não coincidem.");
                }

                return matches;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar senha: {ex.Message}");
                return false;
            }
        }
        public async Task<TokenTemporario> GerarToken2FAAsync(string email)
        {
            // Apaga todos os tokens do mesmo usuário e tipo (2FA)
            var tokensAntigos = _dbContext.TokenTemporario
                .Where(t => t.Email == email && t.Tipo == TipoToken.DoisFatores);
            _dbContext.TokenTemporario.RemoveRange(tokensAntigos);
            await _dbContext.SaveChangesAsync();

            // Gera código 6 dígitos
            var codigo = new Random().Next(100000, 999999).ToString();

            var token = new TokenTemporario
            {
                Tipo = TipoToken.DoisFatores,
                Email = email,
                Codigo = codigo,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddMinutes(15)
            };

            _dbContext.TokenTemporario.Add(token);
            await _dbContext.SaveChangesAsync();

            // Envia o e-mail
            await _emailService.EnviarEmailAsync(email, "Código de Verificação - BitFolio",
                $"Seu código de verificação é: {codigo}. Ele expira em 15 minutos.");

            return token;
        }

        public async Task<string?> ValidarToken2FAAsync(string email, string codigo)
        {
            var token = await _dbContext.TokenTemporario
                .FirstOrDefaultAsync(t => t.Email == email && t.Tipo == TipoToken.DoisFatores);

            if (token == null)
                return null; // Se não encontrar o token, retorna null.

            if (token.Utilizado)
                return null; // Se o token já foi utilizado, retorna null.

            if (DateTime.UtcNow > token.ExpiraEm)
            {
                _dbContext.TokenTemporario.Remove(token);
                await _dbContext.SaveChangesAsync();
                return null; // Se o token expirou, retorna null.
            }

            if (token.Codigo != codigo)
                return null; // Se o código não é válido, retorna null.

            // Se chegou até aqui, o token foi validado com sucesso.
            // Remove o token (uso único)
            _dbContext.TokenTemporario.Remove(token);

            // Gerar o JWT após a validação do 2FA
            var usuario = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email == email);
            if (usuario != null)
            {
                usuario.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                await _dbContext.SaveChangesAsync();
                return GenerateJwtToken(usuario);
            }

            var administrador = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.Email == email);
            if (administrador != null)
            {
                administrador.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                await _dbContext.SaveChangesAsync();
                return GenerateJwtToken(administrador);
            }

            var funcionario = await _dbContext.Recrutadores.FirstOrDefaultAsync(f => f.Email == email);
            if (funcionario != null)
            {
                funcionario.UltimoAcesso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                await _dbContext.SaveChangesAsync();
                return GenerateJwtToken(funcionario);
            }
           
            return null;
        }
        private string GenerateJwtToken(object usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            List<Claim> claims = new List<Claim>();

            if (usuario is Candidato candidato)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, candidato.CandidatoId.ToString()));
                claims.Add(new Claim(ClaimTypes.Email, candidato.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Candidato"));
            }
            else if (usuario is Administrador administrador)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, administrador.AdminId.ToString()));
                claims.Add(new Claim(ClaimTypes.Email, administrador.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
            }
            else if (usuario is Recrutador funcionario)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, funcionario.RecrutadorId.ToString()));
                claims.Add(new Claim(ClaimTypes.Email, funcionario.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Funcionario"));
            }
            else
            {
                throw new ArgumentException("Tipo de usuário inválido.");
            }

            // Cria o token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task EnviarTokenRecuperacaoAsync(string email)
        {
            email = email.ToLower().Trim();

            var usuario = await BuscarUsuarioPorEmail(email);
            if (usuario == null)
                return; 

            var token = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            var entidade = new TokenTemporario
            {
                Email = email,
                Tipo = TipoToken.RecuperacaoSenha,
                Codigo = token,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddMinutes(15),
                Utilizado = false
            };

            _dbContext.TokenTemporario.Add(entidade);
            await _dbContext.SaveChangesAsync();

            await _emailService.EnviarEmailAsync(
                email,
                "Recuperação de Senha",
                $"Seu código de verificação é: {token}"
            );
        }

        public async Task RedefinirSenhaAsync(string email, string token, string novaSenha)
        {
            var recuperacao = await _dbContext.TokenTemporario
                .Where(r => r.Email == email && r.Codigo == token)
                .OrderByDescending(r => r.ExpiraEm)
                .FirstOrDefaultAsync();

            if (recuperacao == null || recuperacao.ExpiraEm < DateTime.UtcNow)
                throw new Exception("Token inválido ou expirado.");

            var usuario = await BuscarUsuarioPorEmail(email);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            var senhaHash = HashPassword(novaSenha);

            if (usuario is Candidato candidato)
                candidato.Senha = senhaHash;
            else if (usuario is Administrador admin)
                admin.Senha = senhaHash;
            else if (usuario is Recrutador func)
                func.Senha = senhaHash;

            _dbContext.TokenTemporario.Remove(recuperacao);
            await _dbContext.SaveChangesAsync();
        }
        // Retorna um dos tipos de usuário
        public async Task<object?> BuscarUsuarioPorEmail(string email)
        {
            email = email.ToLower().Trim();

            var candidato = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email == email);
            if (candidato != null) return candidato;

            var admin = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null) return admin;

            var funcionario = await _dbContext.Recrutadores.FirstOrDefaultAsync(f => f.Email == email);
            if (funcionario != null) return funcionario;

            return null;
        }

        public async Task ConfirmarEmailAsync(string token)
        {
            // Busca o token
            var registro = await _dbContext.TokenTemporario
                .FirstOrDefaultAsync(t => t.Tipo == TipoToken.ValidacaoEmail && t.Codigo == token);

            if (registro == null)
                throw new InvalidOperationException("Token inválido ou não encontrado.");

            // Verifica validade
            if (registro.ExpiraEm < DateTime.UtcNow)
            {
                _dbContext.TokenTemporario.Remove(registro);
                await _dbContext.SaveChangesAsync();

                throw new InvalidOperationException("Token expirado.");
            }

            var candidato = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email == registro.Email);
            var recrutador = await _dbContext.Recrutadores.FirstOrDefaultAsync(r => r.Email == registro.Email);
            var administrador = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.Email == registro.Email);

            if (candidato == null && recrutador == null && administrador == null)
            {
                _dbContext.TokenTemporario.Remove(registro);
                await _dbContext.SaveChangesAsync();

                throw new InvalidOperationException("Usuário não encontrado para o e-mail associado ao token.");
            }

            var agora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            // Ativa e atualiza último acesso
            if (candidato != null)
            {
                candidato.Ativo = true;
                candidato.UltimoAcesso = agora;
            }
            if (recrutador != null)
            {
                recrutador.Ativo = true;
                recrutador.UltimoAcesso = agora;
            }
            if (administrador != null)
            {
                administrador.Ativo = true;
                administrador.UltimoAcesso = agora;
            }

            // Remove o token após confirmação
            _dbContext.TokenTemporario.Remove(registro);

            await _dbContext.SaveChangesAsync();
        }


    }
}
