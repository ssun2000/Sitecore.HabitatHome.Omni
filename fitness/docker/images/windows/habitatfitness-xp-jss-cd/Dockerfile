# escape=`
ARG BASE_IMAGE
ARG JSS_IMAGE

FROM $JSS_IMAGE as jssapp

FROM $BASE_IMAGE as builder

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY *.zip /install/

RUN $env:WDP_PACKAGE = 'HabitatFitness-*.scwdp.zip'; `
    $env:INSTALL_TEMP = 'C:\\install'; `
    Rename-Item -Path (Resolve-Path (Join-Path $env:INSTALL_TEMP $env:WDP_PACKAGE)).Path -NewName 'wdp.zip'; `
    Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/v5.0.2/nuget.exe' -UseBasicParsing -OutFile (Join-Path $env:INSTALL_TEMP '\\nuget.exe'); `
    & (Join-Path $env:INSTALL_TEMP '\\nuget.exe') install 'Microsoft.Web.Xdt' -Version '3.0.0' -OutputDirectory $env:INSTALL_TEMP;

FROM $BASE_IMAGE as final

COPY --from=builder [ "/install/", "/install/" ]
COPY install-package.json /install/
COPY *.ps1 /install/
COPY *.xdt /install/

RUN $env:SITENAME = 'sc'; `
    $env:INSTALL_TEMP = 'C:\\install'; `
    Install-SitecoreConfiguration -Path (Join-Path $env:INSTALL_TEMP '\\install-package.json') -Package (Join-Path $env:INSTALL_TEMP '\\wdp.zip') -Sitename $env:SITENAME;

RUN $env:SITENAME = 'sc'; `
    $env:INSTALL_TEMP = 'C:\\install'; `
    $env:SITE_PATH = 'C:\\inetpub\\{0}' -f $env:SITENAME; `
    $env:XDTDLL_PATH = Join-Path $env:SITE_PATH '\\bin\\Microsoft.Web.XmlTransform.dll'; `
    Move-Item -Path (Join-Path $env:SITE_PATH '\\App_Data\\Transforms\\HabitatFitness-*.sccpl') -Destination (Join-Path $env:INSTALL_TEMP '\\HabitatFitness-sccpl.zip'); `
    Expand-Archive -Path (Join-Path $env:INSTALL_TEMP '\\HabitatFitness-sccpl.zip') -DestinationPath (Join-Path $env:INSTALL_TEMP '\\HabitatFitness-sccpl'); `
    Copy-Item -Path (Join-Path $env:INSTALL_TEMP "\\Microsoft.Web.Xdt.*\\lib\\net40\\Microsoft.Web.XmlTransform.dll") -Destination $env:XDTDLL_PATH; `
    & (Join-Path $env:INSTALL_TEMP "\\Invoke-XdtTransform.ps1") -Path (Join-Path $env:SITE_PATH '\\Web.config') -XdtPath (Join-Path $env:INSTALL_TEMP '\\HabitatFitness-sccpl\\Xdts\\Web.config.xdt') -XdtDllPath $env:XDTDLL_PATH; `
    & (Join-Path $env:INSTALL_TEMP "\\Invoke-XdtTransform.ps1") -Path (Join-Path $env:SITE_PATH '\\App_Config\\ConnectionStrings.config') -XdtPath (Join-Path $env:INSTALL_TEMP '\\ConnectionStrings.config.xdt') -XdtDllPath $env:XDTDLL_PATH; `
    $env:IIS_PATH = 'IIS:\Sites\{0}' -f $env:SITENAME; `
    Clear-WebConfiguration -PSPath $env:IIS_PATH -Filter '/system.webserver/rewrite/rules/rule'; `
    New-Item -Path (Join-Path $env:SITE_PATH '\\upload') -ItemType 'Directory' | Out-Null; `
    Remove-Item -Path $env:INSTALL_TEMP -Force -Recurse;

COPY --from=jssapp /deploy /inetpub/sc/. 