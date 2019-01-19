// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace Chromely.CefSharp.Win
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using Chromely.CefGlue.Winapi;
    using Chromely.CefGlue.Winapi.ChromeHost;
    using Chromely.Core;
    using Chromely.Core.Helpers;
    using Chromely.Core.Infrastructure;
    using WinApi.Windows;

    class Program
    {
        static void Main(string[] args)
        {
           try
            {
                HostHelpers.SetupDefaultExceptionHandlers();

                /*
                * Start url (load html) options:
                */

                // Options 1 - real standard urls 
                // string startUrl = "https://google.com";

                // Options 2 - using local resource file handling with default/custom local scheme handler 
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource scheme handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                string startUrl = "local://app/chromely.html";

                // Options 3 - using file protocol - using default/custom scheme handler for Ajax/Http requests
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                // Requires - (sample) UseDefaultHttpSchemeHandler("http", "chromely.com")
                //            or register new htpp scheme handler - RegisterSchemeHandler("htpp", "test.com",  new CustomHttpHandler())
                // string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                // string startUrl = $"file:///{appDirectory}app/chromely_with_ajax.html";

                ChromelyConfiguration config = ChromelyConfiguration
                                              .Create()
                                              .WithAppArgs(args)
                                              .WithHostSize(1200, 900)
                                              .WithLogFile("logs\\chromely.cef_new.log")
                                              .WithStartUrl(startUrl)
                                              .WithLogSeverity(LogSeverity.Info)
                                              .UseDefaultLogger("logs\\chromely_new.log")
                                              .UseDefaultResourceSchemeHandler("local", string.Empty)
                                              .UseDefaultHttpSchemeHandler("http", "chromely.com")

                                              // The single process should only be used for debugging purpose.
                                              // For production, this should not be needed when the app is published and an cefglue_winapi_netcoredemo.exe 
                                              // is created.

                                              // Alternate approach for multi-process, is to add a subprocess application
                                              // .WithCustomSetting(CefSettingKeys.BrowserSubprocessPath, full_path_to_subprocess)
                                              .WithCustomSetting(CefSettingKeys.SingleProcess, true);

                var factory = WinapiHostFactory.Init("chromely.ico");
                using (var window = factory.CreateWindow(
                    () => new CefGlueBrowserHost(config),
                    "chromely",
                    constructionParams: new FrameWindowConstructionParams()))
                {
                    // Register external url schems
                    window.RegisterUrlScheme(new UrlScheme("https://github.com/chromelyapps/Chromely", true));
                    
                    // window.RegisterUrlScheme(new UrlScheme("https://google.com", true));

                    /*
                     * Register service assemblies
                     * Uncomment relevant part to register assemblies
                     */

                    // 1. Register current/local assembly:
                    window.RegisterServiceAssembly(Assembly.GetExecutingAssembly());

                    // 2. Register external assembly with file name:
                    // string serviceAssemblyFile = @"C:\ChromelyDlls\Chromely.Service.Demo.dll";
                    // string serviceAssemblyFile = @"Chromely.Service.Demo.dll";
                    // window.RegisterServiceAssembly(serviceAssemblyFile);

                    // 3. Register external assemblies with list of filenames:
                    // string serviceAssemblyFile1 = @"C:\ChromelyDlls\Chromely.Service.Demo.dll";
                    // List<string> filenames = new List<string>();
                    // filenames.Add(serviceAssemblyFile1);
                    // window.RegisterServiceAssemblies(filenames);

                    // 4. Register external assemblies directory:
                    string serviceAssembliesFolder = @"C:\ChromelyDlls";
                    window.RegisterServiceAssemblies(serviceAssembliesFolder);

                    // Scan assemblies for Controller routes 
                    window.ScanAssemblies();

                    window.SetSize(config.HostWidth, config.HostHeight);
                    window.CenterToScreen();
                    window.Show();
                    
                    new HostEventLoop().Run(window);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
    }
}
