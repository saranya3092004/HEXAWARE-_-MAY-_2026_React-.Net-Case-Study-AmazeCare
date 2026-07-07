namespace AmazeCare.Server.Modules.Extensions
{
    public static class CorsSetupExtensions
    {
        public static string PolicyName => "AmazeCareCorsPolicy";

        public static IServiceCollection AddAmazeCareCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
