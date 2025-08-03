using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using ProyectoDistri2.DAL;
using System.Linq;

public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
{
    public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    {
        context.Validated(); // No validamos el cliente en este ejemplo
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    {
        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

        using (var db = new GestorReserva())
        {
            // Buscar usuario por nombre y contraseña
            var user = db.Usuarios.FirstOrDefault(u =>
                u.Nombre == context.UserName &&
                u.Password == context.Password
            );

            if (user == null)
            {
                context.SetError("invalid_grant", "Usuario o contraseña incorrectos.");
                return;
            }

            // Crear identidad con nombre y rol
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Nombre));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Rol));

            context.Validated(identity);
        }
    }
}
