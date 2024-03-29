# 9.1.0
- Add `MonoBheaviourInjected` again with new implementation. Extend docs accordingly

# 9.0.2
- Fix override handling when running the editor tool

# 9.0.1
- Fix Serilog Template

# 9.0.0
- Remove `Injecter.Hosting.Unity`
- Rework `Injecter.Unity` now it uses `MonoInjector` and `MonoDisposer`
- `IScopeStore` now accepts objects instead of generics
- `Injecter` now returns null if the target type has no injectable members
- `Injecter` injects faster and with less allocations
- `IInjecter`'s generic overload is removed
- New `AppInstaller` templates

# 8.0.1
- Move AppInstallerTemplateItem to editor folder
  
# 8.0.0
- Unity lifetime clears scopes

# 7.1.0
- Add appsettings file generation template buttons

# 7.0.2
- Add missing meta file

# 7.0.1
- Fix `MonoBehaviourInjected` and `MonoBehaviourScoped` and separate them to different files

# 7.0.0
- Add `MonoBehaviourInjected` and `MonoBehaviourScoped`
- Renamed classes: \*MonoBehavior\* -> \*MonoBehavio**u**r\*
- InjectedMonoBehavior now removes Scope from the store during `OnDestroy`
- ScopeStore is now generic
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
