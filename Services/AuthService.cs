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
                DtCriacao = agora
            };

            _dbContext.Candidatos.Add(candidato);
            await _dbContext.SaveChangesAsync();

            return candidato;
        }

        public async Task<string?> LoginAsync(string email, string senha)
        {
            var usuario = await _dbContext.Candidatos.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (usuario == null)
                return null;

            if (!VerifyPassword(senha, usuario.SenhaHash))
                return null;

            var token = GenerateJwtToken(usuario);

            // TODO: log login (pode ser implementado)

            return token;
        }

        // Hash password with salt using PBKDF2
        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            var result = new byte[49];
            result[0] = 0x00; // marker
            Buffer.BlockCopy(salt, 0, result, 1, 16);
            Buffer.BlockCopy(hash, 0, result, 17, 32);

            return Convert.ToBase64String(result);
        }

        // Verify a password with the hashed stored password
        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var decoded = Convert.FromBase64String(storedHash);

                if (decoded.Length != 49 || decoded[0] != 0x00)
                    return false;

                var salt = new byte[16];
                Buffer.BlockCopy(decoded, 1, salt, 0, 16);

                var hash = new byte[32];
                Buffer.BlockCopy(decoded, 17, hash, 0, 32);

                var testHash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32);

                return CryptographicOperations.FixedTimeEquals(hash, testHash);
            }
            catch
            {
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

        private string GenerateJwtToken(Candidato usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.GetType().Name)
            };

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
