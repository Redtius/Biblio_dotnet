using System.ComponentModel.DataAnnotations;


namespace Biblio.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Veuillez entrer un email valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

