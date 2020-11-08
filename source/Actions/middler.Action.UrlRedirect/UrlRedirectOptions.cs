namespace middler.Action.UrlRedirect
{
    public class UrlRedirectOptions
    {
        public string RedirectTo { get; set; }
        public bool Permanent { get; set; }
        public bool PreserveMethod { get; set; } = true;
    }
}