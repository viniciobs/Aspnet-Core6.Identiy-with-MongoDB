$OutputDirectory = "./testcoveragereport"

Write-Host "Deleting older reports..."
Remove-Item $OutputDirectory -Recurse -ErrorAction SilentlyContinue

Write-Host "Deleting older tests..."
foreach ($Project in Get-ChildItem *.Tests.csproj -Recurse | Select-Object)
{
    Write-Host $Project.Name
    Remove-Item "$($Project.Directory)/TestResults" -Recurse -ErrorAction SilentlyContinue
}

Write-Host "Installing dotnet-reportgenerator-globaltool..."
dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null

Write-Host "Collecting data..."
dotnet test --collect:"XPlat Code Coverage" | Out-Null

Write-Host "Generating report..."
foreach ($TestFile in Get-ChildItem coverage.cobertura.xml -Recurse | Select-Object)
{
    reportgenerator -reports:$TestFile.FullName -targetdir:$OutputDirectory -reporttypes:Html | Out-Null
}

Write-Host "Opening file results in browser..."
Invoke-Expression "$($OutputDirectory)/index.html"

Write-Host "Finished"

