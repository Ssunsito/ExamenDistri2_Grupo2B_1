/* *****************************************************
Proyecto Distri2 - Reserva.co
Archivo SimpleAuthorizationServerProvider.cs
Kenneth Andrés Pantoja Manobanda
Santiago Pila
Fecha: 03 / 08 / 2025

// RESULTADOS FINALES
- Se implementó proveedor de autenticación OAuth personalizado
- Se integró validación de credenciales de usuario
- Se estableció gestión de claims para identidad
- Se implementó manejo de errores de autenticación
- Se centralizó la autenticación de recursos propietarios

// CONCLUSIONES
1. El proveedor proporciona una autenticación segura y eficiente
2. La gestión de claims permite una identificación precisa de usuarios
3. El sistema de validación mejora la seguridad del sistema
4. La integración con la base de datos optimiza el acceso a datos
5. La estructura del código facilita el mantenimiento y extensión
*********************************************************************** */


using Microsoft.Owin.Security.OAuth;
using ProyectoDistri2.DAL;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
