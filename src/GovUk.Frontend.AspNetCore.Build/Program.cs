using GovUk.Frontend.AspNetCore.Build;

var downloader = new NpmPackageDownloader();

await downloader.DownloadPackage(
    "govuk-frontend",
    "5.10.2",
    "/Users/james/govuk-frontend",
    "dist/govuk",
    ["assets/**"]);
