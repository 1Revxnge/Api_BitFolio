using System.ComponentModel.DataAnnotations;

namespace ApiJobfy.models
{
    public class Administrador : Funcionario
    {
        // Administrador herda Funcionario e adiciona permissões especiais
        // Pode implementar permissões RBAC ou campos específicos se desejar
    }
}
