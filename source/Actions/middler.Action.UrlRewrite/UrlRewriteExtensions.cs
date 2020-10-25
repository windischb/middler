using middler.Common.Interfaces;

namespace middler.Action.UrlRewrite
{
    public static class UrlRewriteExtensions
    {

        public static void RewriteTo(this IMiddlerMapActionsBuilder builder, UrlRewriteOptions options)
        {
            builder.AddAction<UrlRewriteAction, UrlRewriteOptions>(options);
        }

        public static void RewriteTo(this IMiddlerMapActionsBuilder builder, string rewriteTo)
        {
            var opts = new UrlRewriteOptions();
            opts.RewriteTo = rewriteTo;

            RewriteTo(builder, opts);
        }

    }
}