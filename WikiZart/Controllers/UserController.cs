using System;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using System.Web.Security;
using System.Security.Permissions;


namespace WikiZart.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        OpenIdRelyingParty openId;

        public UserController()
        {
            openId = new OpenIdRelyingParty();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.Redirect(FormsAuthentication.DefaultUrl);
        }

        private ActionResult SendRequestToOpenIdProvider(string identifier)
        {
            Identifier id;
            if (Identifier.TryParse(identifier, out id))
            {
                try
                {
                    IAuthenticationRequest request = openId.CreateRequest(Identifier.Parse(identifier));
                    request.AddExtension(new ClaimsRequest
                    {
                        Email = DemandLevel.Require,
                        Nickname = DemandLevel.Require,
                        FullName = DemandLevel.Request
                    });
                    return request.RedirectingResponse.AsActionResult();
                }
                catch (ProtocolException e)
                {
                    ModelState.AddModelError("openid_identifier", e);
                }
            }
            else
            {
                ModelState.AddModelError("openid_identifier", "Invalid identifier");
            }

            return View();
        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get), ValidateInput(false)]
        public ActionResult Login(string openid_identifier, string returnUrl)
        {
            var response = openId.GetResponse();
            if (response == null)
            {
                if (string.IsNullOrEmpty(openid_identifier))
                    return View();

                return SendRequestToOpenIdProvider(openid_identifier);
            }
            else
            {

                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        string identifier = response.ClaimedIdentifier;

                        var simpleReg = response.GetExtension<ClaimsResponse>();
                        string email = "";
                        string fullname;
                        string nickname;
                        if (simpleReg != null)
                        {
                            if (!string.IsNullOrEmpty(simpleReg.Email))
                                email = simpleReg.Email;

                            if (!string.IsNullOrEmpty(simpleReg.FullName))
                                fullname = simpleReg.FullName;

                            if (!string.IsNullOrEmpty(simpleReg.Nickname))
                                nickname = simpleReg.Nickname;
                        }
                        var authTicket = new FormsAuthenticationTicket( 
                            1,                             // version 
                            email,                      // user name 
                            DateTime.Now,                  // created 
                            DateTime.Now.AddMinutes(20),   // expires 
                            true,                    // persistent? 
                            "Contributor"                        // can be used to store roles 
                            ); 
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket); 
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket); 
                        Response.Cookies.Add(authCookie);

                        return Redirect(System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"] + returnUrl);


                    case AuthenticationStatus.Canceled:
                        ModelState.AddModelError("openid_identifier", "Authetication canceled");
                        break;
                    case AuthenticationStatus.Failed:
                        ModelState.AddModelError("openid_identifier", "Authetication failed");
                        break;
                }
            }
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["ApplicationUrl"] + returnUrl);
        }

    }
}
