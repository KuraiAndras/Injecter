# 7.0.0
- Update dependencies
- Update minimum Unity version to `2020.3`
- Update to .NET 6 where not breaking

# 6.1.2
- `UseUnity` requires all parameters specified

# 6.1.1
- Use `ConfigureHostConfiguration` instead of `ConfigureAppConfiguration`

# 6.1.0
- Improved generic host experience with unity
- Extension methods to do registrations on scene load events
- Updated dependencies

# 6.0.1
- Don't throw when GameObject has a null component

# 6.0.0

- Removed Injecter.WPF inheritable classes
- When using the InjectScoped property you can get the create scope via the IScopeStore interface
- Removed Injecter.UWP inheritable classes
- Added attached property injection to Injecter.UWP
- Removed Injecter.Xamarin.Forms inheritable classes
- Added attached property injection to Injecter.Xamarin.Forms
- Changed default scoping to not create scopes
- Injecter.Unity now exposes scope creation options properly
