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
            byte[] curriculoBytes;
            using (var ms = new MemoryStream())
            {
                await dto.CurriculoPdf.CopyToAsync(ms);
                curriculoBytes = ms.ToArray(); // Obtém os bytes do arquivo PDF
            }
            var candidato = new Candidato
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = senhaHash,
                DtNascimento = dto.DataNascimento,
                Telefone = dto.Telefone,
                CurriculoCriptografado = curriculoBytes,
                DtCriacao = agora,
                Status = "Aprovado",
                Ativo =  true

            };

            _dbContext.Candidatos.Add(candidato);
            await _dbContext.SaveChangesAsync();

            return candidato;
        }
        public async Task<Funcionario> RegisterFuncionarioAsync(RegisterFuncionarioDto dto)
        {
            string senhaHash = HashPassword(dto.Senha);
            DateTime agora = DateTime.Now;

            var funcionario = new Funcionario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = senhaHash,
                Telefone = dto.Telefone,
                DtNascimento = dto.DataNascimento,
                Cargo = dto.Cargo,
                NegocioId = dto.NegocioId,
                StatusFunc = "Aprovado",
                DtAdmissao = dto.DtAdmissao,
                DtCriacao = agora,
                Ativo = true,
                
            };

            _dbContext.Funcionarios.Add(funcionario);
            await _dbContext.SaveChangesAsync();

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
                DtNascimento = dto.DataNascimento,
                Cargo = dto.Cargo,
                NegocioId = dto.NegocioId,
                Aprovado = true,
                DtCadastro = agora,
                DtAprovacao = DateOnly.FromDateTime(agora)

            };

            _dbContext.Administradores.Add(administrador);
            await _dbContext.SaveChangesAsync();

            return administrador;
        }

        public async Task<string?> LoginAsync(string email, string senha, string tipo)
        {
            email = email.ToLower().Trim();
            senha = senha.Trim();

            switch (tipo.ToLower())
            {
                case "candidato":
                    var candidato = await _dbContext.Candidatos.FirstOrDefaultAsync(c => c.Email == email);
                    if (candidato == null)
                        throw new InvalidOperationException("Candidato não encontrado.");

                    if (await EstaBloqueadoPorEmailCandidato(email))
                        throw new InvalidOperationException("Este candidato está temporariamente bloqueado por várias tentativas falhas.");

                    bool senhaValidaCandidato = VerifyPassword(senha, candidato.SenhaHash);
                    await RegistrarLogCandidato(candidato.Id, senhaValidaCandidato);

                    if (!senhaValidaCandidato)
                    {
                        int restantes = await TentativasRestantesCandidato(candidato.Id);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }

                    await LimparTentativasFalhasCandidato(candidato.Id);
                    return GenerateJwtToken(candidato);

                case "administrador":
                    var administrador = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.Email == email);
                    if (administrador == null)
                        throw new InvalidOperationException("Administrador não encontrado.");

                    if (await EstaBloqueadoPorEmailAdministrador(email))
                        throw new InvalidOperationException("Administrador bloqueado por muitas tentativas falhas.");

                    bool senhaValidaAdmin = VerifyPassword(senha, administrador.Senha);
                    await RegistrarLogAdministrador(administrador.Id, senhaValidaAdmin);

                    if (!senhaValidaAdmin)
                    {
                        int restantes = await TentativasRestantesAdministrador(administrador.Id);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }

                    await LimparTentativasFalhasAdministrador(administrador.Id);
                    return GenerateJwtToken(administrador);

                case "funcionario":
                    var funcionario = await _dbContext.Funcionarios.FirstOrDefaultAsync(f => f.Email == email);
                    if (funcionario == null)
                        throw new InvalidOperationException("Funcionário não encontrado.");

                    if (await EstaBloqueadoPorEmailFuncionario(email))
                        throw new InvalidOperationException("Funcionário bloqueado temporariamente.");

                    bool senhaValidaFunc = VerifyPassword(senha, funcionario.Senha);
                    await RegistrarLogFuncionario(funcionario.Id, senhaValidaFunc);

                    if (!senhaValidaFunc)
                    {
                        int restantes = await TentativasRestantesFuncionario(funcionario.Id);
                        throw new InvalidOperationException($"Credenciais inválidas. Tentativas restantes: {restantes}");
                    }

                    await LimparTentativasFalhasFuncionario(funcionario.Id);
                    return GenerateJwtToken(funcionario);

                default:
                    throw new InvalidOperationException("Tipo de usuário inválido.");
            }
        }

        private async Task RegistrarLogCandidato(int candidatoId, bool sucesso)
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
                    DataAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogCandidatos.Add(new LogCandidato
                {
                    CandidatoId = candidatoId,
                    Acao = "Login falhou",
                    DataAcao = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task RegistrarLogAdministrador(int adminId, bool sucesso)
        {
            if (sucesso)
            {
                var logsSucesso = await _dbContext.LogAdministrador
                    .Where(l => l.AdministradorId == adminId && l.Acao == "Login bem-sucedido")
                    .ToListAsync();

                if (logsSucesso.Any())
                    _dbContext.LogAdministrador.RemoveRange(logsSucesso);

                _dbContext.LogAdministrador.Add(new LogAdministrador
                {
                    AdministradorId = adminId,
                    Acao = "Login bem-sucedido",
                    DtAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogAdministrador.Add(new LogAdministrador
                {
                    AdministradorId = adminId,
                    Acao = "Login falhou",
                    DtAcao = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();
        }


        private async Task RegistrarLogFuncionario(int funcionarioId, bool sucesso)
        {
            if (sucesso)
            {
                var logsSucesso = await _dbContext.LogFuncionarios
                    .Where(l => l.FuncionarioId == funcionarioId && l.Acao == "Login bem-sucedido")
                    .ToListAsync();

                if (logsSucesso.Any())
                    _dbContext.LogFuncionarios.RemoveRange(logsSucesso);

                _dbContext.LogFuncionarios.Add(new LogFuncionarios
                {
                    FuncionarioId = funcionarioId,
                    Acao = "Login bem-sucedido",
                    DtAcao = DateTime.UtcNow
                });
            }
            else
            {
                _dbContext.LogFuncionarios.Add(new LogFuncionarios
                {
                    FuncionarioId = funcionarioId,
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
                .Where(l => l.CandidatoId == candidato.Id && l.DataAcao >= limite && l.Acao == "Login falhou")
                .CountAsync();

            return falhas >= 5;
        }

        private async Task<bool> EstaBloqueadoPorEmailAdministrador(string email)
        {
            var admin = await _dbContext.Administradores.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (admin == null)
                return false;

            var limite = DateTime.UtcNow.AddMinutes(-15);

            var logs = await _dbContext.LogAdministrador
                .Where(l => l.AdministradorId == admin.Id && l.DtAcao >= limite && l.Acao.Contains("Login falhou"))
                .OrderByDescending(l => l.DtAcao)
                .Take(5)
                .ToListAsync();

            return logs.Count >= 5;
        }

        private async Task<bool> EstaBloqueadoPorEmailFuncionario(string email)
        {
            var func = await _dbContext.Funcionarios.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
            if (func == null)
                return false;
            var limite = DateTime.UtcNow.AddMinutes(-15);

            var logs = await _dbContext.LogFuncionarios
                .Where(l => l.FuncionarioId == func.Id && l.DtAcao >= limite && l.Acao.Contains("Login falhou"))
                .OrderByDescending(l => l.DtAcao)
                .Take(5)
                .ToListAsync();

            return logs.Count >= 5;
        }
        private async Task<int> TentativasRestantesCandidato(int candidatoId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogCandidatos
                .CountAsync(l => l.CandidatoId == candidatoId && l.DataAcao >= desde && l.Acao == "Login falhou");

            return Math.Max(0, 5 - falhas);
        }

        private async Task<int> TentativasRestantesAdministrador(int adminId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogAdministrador
                .CountAsync(l => l.AdministradorId == adminId && l.DtAcao >= desde && l.Acao.Contains("Login falhou"));

            return Math.Max(0, 5 - falhas);
        }

        private async Task<int> TentativasRestantesFuncionario(int funcionarioId)
        {
            var desde = DateTime.UtcNow.AddMinutes(-15);
            var falhas = await _dbContext.LogFuncionarios
                .CountAsync(l => l.FuncionarioId == funcionarioId && l.DtAcao >= desde && l.Acao.Contains("Login falhou"));

            return Math.Max(0, 5 - falhas);
        }

        private async Task LimparTentativasFalhasCandidato(int candidatoId)
        {
            var logsFalhos = _dbContext.LogCandidatos
                .Where(l => l.CandidatoId == candidatoId && l.Acao == "Login falhou");

            _dbContext.LogCandidatos.RemoveRange(logsFalhos);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LimparTentativasFalhasAdministrador(int adminId)
        {
            var logsFalhos = _dbContext.LogAdministrador
                .Where(l => l.AdministradorId == adminId && l.Acao == "Login falhou");

            _dbContext.LogAdministrador.RemoveRange(logsFalhos);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LimparTentativasFalhasFuncionario(int funcionarioId)
        {
            var logsFalhos = _dbContext.LogFuncionarios
                .Where(l => l.FuncionarioId == funcionarioId && l.Acao == "Login falhou");

            _dbContext.LogFuncionarios.RemoveRange(logsFalhos);
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

        // Verify a password with the hashed stored password
        private bool VerifyPassword(string password, string storedHash)
        {
            password = password.Trim();

            try
            {
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

                Console.WriteLine($"Salt: {BitConverter.ToString(salt)}");
                Console.WriteLine($"Stored Hash: {BitConverter.ToString(hash)}");
                Console.WriteLine($"Test Hash:   {BitConverter.ToString(testHash)}");

                bool matches = CryptographicOperations.FixedTimeEquals(hash, testHash);

                if (!matches)
                {
                    Console.WriteLine("❌ Hashes não coincidem.");
                }

                return matches;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar senha: {ex.Message}");
                return false;
            }
        }
        // Encrypt data symmetrically using AES with a fixed key (must be safe key)
        private byte[] EncryptData(byte[] data)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"].PadRight(32).Substring(0, 32));
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        private string GenerateJwtToken(object usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            List<Claim> claims = new List<Claim>();

            if (usuario is Candidato candidato)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, candidato.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Email, candidato.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Candidato"));
            }
            else if (usuario is Administrador administrador)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Email, administrador.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
            }
            else if (usuario is Funcionario funcionario)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, funcionario.Id.ToString()));
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
                return; // Não revela se existe

            var token = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            var entidade = new CodigoRecuperacaoSenha
            {
                Email = email,
                Codigo = token,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddMinutes(15),
                Utilizado = false
            };

            _dbContext.CodigosRecuperacaoSenha.Add(entidade);
            await _dbContext.SaveChangesAsync();

            // Simule envio de e-mail
            await _emailService.EnviarEmailAsync(email, "Recuperação de Senha", $"Seu código de verificação é: {token}");
        }

        public async Task RedefinirSenhaAsync(string email, string token, string novaSenha)
        {
            var recuperacao = await _dbContext.CodigosRecuperacaoSenha
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
                candidato.SenhaHash = senhaHash;
            else if (usuario is Administrador admin)
                admin.Senha = senhaHash;
            else if (usuario is Funcionario func)
                func.Senha = senhaHash;

            _dbContext.CodigosRecuperacaoSenha.Remove(recuperacao);
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

            var funcionario = await _dbContext.Funcionarios.FirstOrDefaultAsync(f => f.Email == email);
            if (funcionario != null) return funcionario;

            return null;
        }
    }
}
