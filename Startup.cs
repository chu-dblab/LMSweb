using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(LMSweb.Startup))]

namespace LMSweb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 如需如何設定應用程式的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=316888

            // 說明：
            //      利用AuthenticationType欄位來區分登入者是老師還是學生
            //      所以要寫分開的2個UseCookieAuthentication
            //      實務上很少這樣做，有牽涉到身分(Role)就會像之前一樣登入頁只會有一個，再透過資料庫查詢出登入的使用者是什麼身分
            //      也方便和Google、FB...等服務做身分驗證API串接

            /*app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Student",
                LoginPath = new PathString("/Home/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30)   //設置cookie保留的時間
            });*/

            /* app.UseCookieAuthentication(new CookieAuthenticationOptions
             {
                 AuthenticationType = "Teacher",
                 LoginPath = new PathString("/Home/Login"),
                 ExpireTimeSpan = TimeSpan.FromMinutes(30)
             });*/

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Home/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30)   //設置cookie保留的時間
            });
            app.MapSignalR();
        }
    }
}