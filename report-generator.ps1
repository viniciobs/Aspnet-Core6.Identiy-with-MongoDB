$OutputDirectory = "./testcoveragereport"

Write-Host "Deleting old reports..." -ForegroundColor Green
Remove-Item $OutputDirectory -Recurse -ErrorAction SilentlyContinue

Write-Host "Deleting old tests..." -ForegroundColor Green
foreach ($Project in Get-ChildItem *.Tests.csproj -Recurse | Select-Object)
{
    Remove-Item "$($Project.Directory)/TestResults" -Recurse -ErrorAction SilentlyContinue
}

Write-Host "Installing dotnet-reportgenerator-globaltool..." -ForegroundColor Green
dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null

Write-Host "Collecting data..." -ForegroundColor Green
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --collect:"XPlat Code Coverage" | Out-Null

Write-Host "Generating report..." -ForegroundColor Green
$TestResults = Get-ChildItem coverage.cobertura.xml -Recurse | Select-Object FullName
$TestFiles = ""
for ($i = 0; $i -lt $TestResults.Length; $i++)
{
    if ($i -gt 0)
    {
        $TestFiles += ';'
    }

    $TestFiles += $TestResults[$i].FullName
}
reportgenerator -reports:$TestFiles -targetdir:$OutputDirectory -reporttypes:Html -assemblyfilters:"-Ioc;-Infra" -classfilters:-Program.cs | Out-Null

Write-Host "Opening file results in browser..." -ForegroundColor Green
Invoke-Expression "$($OutputDirectory)/index.html"

Write-Host "Finished" -ForegroundColor Green

