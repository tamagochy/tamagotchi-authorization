
namespace tamagotchi_authorization
{
    /// <summary>
    /// Constants
    /// </summary>
    internal class Scope
    {
        public const string SecurityKey = "TamagochiSecretKey";
        //Время жизни токена в часах
        public const int LifeTime = 24;
        //Почта приложения
        public const string ApplicationMail = "tamagotchi.vlsu@gmail.com";
        //Пароль от почты
        public const string MailPassword= "istm.prim.2018";
    }
}
