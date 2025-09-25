namespace ignivault.ApiClient
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenManager _tokenManager;

        public AuthHeaderHandler(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Sends an HTTP request with an Authorization header containing a Bearer token.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenManager.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
