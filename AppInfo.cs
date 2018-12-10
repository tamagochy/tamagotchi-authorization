
namespace Tamagotchi.Authorization
{
    public class AppInfo
    {
        //Время жизни токена в часах
        public int LifeTimeToken {get;set;}
        public int ConfirmCodeLifeTime { get; set; }
        public int CountRound { get; set; }
        public string ApplicationUrl { get; set; }
        public string ProjectVersion { get; set; }
        public string SecretKey { get; set; }
        //Почта приложения
        public string ApplicationEmail { get; set; } 
        //Пароль от почты
        public string EmailPassword { get; set; }
    }
}
