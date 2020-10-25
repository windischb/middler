using middler.Common.Interfaces;

namespace middler.Common.Actions.UrlRedirect
{
    public static class UrlRedirectionExtensions
    {

        public static void RedirectTo(this IMiddlerMapActionsBuilder builder, UrlRedirectOptions options)
        {
            builder.AddAction<UrlRedirectAction, UrlRedirectOptions>(options);
        }

        public static void RedirectTo(this IMiddlerMapActionsBuilder builder, string redirectTo, bool permanent = false)
        {
            var opts = new UrlRedirectOptions();
            opts.RedirectTo = redirectTo;
            opts.Permanent = permanent;

            RedirectTo(builder, opts);
        }

    }
}