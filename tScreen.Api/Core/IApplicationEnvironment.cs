namespace Core
{
    public interface IApplicationEnvironment
    {
        /// <summary>
        /// Environment name set on environment variable ASPNETCORE_ENVIRONMENT
        /// </summary>
        string EnvironmentName { get; set; }
        
        /// <summary>
        /// Reads the value from ASPNETCORE_ENVIRONMENT and compares against method parameter
        /// </summary>
        /// <param name="name">Environment name e.g, Development, Staging, Production, Custom</param>
        bool IsEnvironment(string name);
        
        /// <summary>
        /// Check if the environment is development
        /// </summary>
        bool IsDevelopment();
        
        /// <summary>
        /// Check if the environment is testing
        /// </summary>
        bool IsTesting();
        
        /// <summary>
        /// Check if the environment is staging
        /// </summary>
        bool IsStaging();
        
        /// <summary>
        /// Check if the environment is production
        /// </summary>
        bool IsProduction();

        /// <summary>
        /// Check if the host is locally hosted. 
        /// </summary>
        /// <remarks>
        /// If the environment variable `APPLICATION_HOST` is equal to "Local" then returns true
        /// any other environment is false
        /// </remarks>
        /// <returns></returns>
        public bool IsLocallyHosted();
        
        /// <summary>
        /// Checks the environment variable APPLICATION_HOST to see if it's equal to "Container."
        /// In a Docker or containerized environment like Kubernetes, APPLICATION_HOST is set to container
        /// else is undefined or set to "Local" which represents a local development environment.  
        /// </summary>
        bool IsContainerHosted();
        
        /// <summary>
        /// Covers application environments Development and Testing
        /// </summary>
        bool IsKnownUnprotectedEnvironment();

        /// <summary>
        /// Check to see if the environment variable `APPLICATION_USE_DEVELOPMENT_SERVICES`
        /// <see cref="EnvironmentVariableNames"/> is set to true
        /// </summary>
        /// <returns>true or false</returns>
        bool ShouldUseLocalDevelopmentServices();
        
        /// <summary>
        /// Covers application environments Staging and Production
        /// </summary>
        bool IsKnownProtectedEnvironment();
        
        /// <summary>
        /// Get any environment variable or default if variable doesn't exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetEnvironmentVariableOrDefault<T>(string key, T defaultValue);
    }
}