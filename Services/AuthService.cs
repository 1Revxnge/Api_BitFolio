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

        public AuthService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto)
        {
            // Hash password
            string senhaHash = HashPassword(dto.Senha);
            DateTime agora = DateTime.Now;
            // Encrypt PDF (currículo)
            byte[] curriculoBytes;
            using (var ms = new MemoryStream())
            {
                await dto.CurriculoPdf.CopyToAsync(ms);
                curriculoBytes = ms.ToArray();
            }
            var curriculoCriptografado = EncryptData(curriculoBytes);

            var candidato = new Candidato
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = senhaHash,
                DtNascimento = dto.DataNascimento,
                Telefone = dto.Telefone,
                CurriculoCriptografado = curriculoCriptografado,
                DtCriacao = agora,
                Status = "Aprovado"
            };

            _dbContext.Candidatos.Add(candidato);
            await _dbContext.SaveChangesAsync();

            return candidato;
        }

        public async Task<string?> LoginAsync(string email, string senha)
        {
            TestarHashing();

            // Tenta buscar o usuário nas 3 tabelas (Candidatos, Administradores e Funcionarios)
            var candidato = await _dbContext.Candidatos
                                            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            senha = senha.Trim();
            if (candidato != null)
            {
                if (!VerifyPassword(senha, candidato.SenhaHash))
                    return null;  // Senha inválida

                var token = GenerateJwtToken(candidato);
                // TODO: log login (pode ser implementado)
                return token;
            }

            var administrador = await _dbContext.Administradores
                                                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());

            if (administrador != null)
            {
                if (!VerifyPassword(senha, administrador.Senha))
                    return null;  // Senha inválida

                var token = GenerateJwtToken(administrador);
                // TODO: log login (pode ser implementado)
                return token;
            }

            var funcionario = await _dbContext.Funcionarios
                                              .FirstOrDefaultAsync(f => f.Email.ToLower() == email.ToLower());

            if (funcionario != null)
            {
                if (!VerifyPassword(senha, funcionario.Senha))
                    return null;  // Senha inválida

                var token = GenerateJwtToken(funcionario);
                // TODO: log login (pode ser implementado)
                return token;
            }

            // Se nenhum usuário foi encontrado, retorna null
            return null;
        }

        public void TestarHashing()
        {
            string senhaTeste = "12345678";
            string hashGerado = HashPassword(senhaTeste);
            bool resultadoVerificacao = VerifyPassword(senhaTeste, hashGerado);
            Console.WriteLine($"Hash gerado: {hashGerado}");
            Console.WriteLine($"Verificação: {resultadoVerificacao}");
            return;
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

    }
}
