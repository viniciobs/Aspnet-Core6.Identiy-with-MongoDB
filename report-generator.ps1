$OutputDirectory = "./testcoveragereport"

Write-Host "Deleting older reports..."
Remove-Item $OutputDirectory -Recurse -ErrorAction SilentlyContinue

Write-Host "Deleting older tests..."
foreach ($Project in Get-ChildItem *.Tests.csproj -Recurse | Select-Object)
{
    Remove-Item "$($Project.Directory)/TestResults" -Recurse -ErrorAction SilentlyContinue
}

Write-Host "Installing dotnet-reportgenerator-globaltool..."
dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null

Write-Host "Collecting data..."
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:ExcludeByFile="**/Program.cs" --collect:"XPlat Code Coverage" | Out-Null

Write-Host "Generating report..."
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
reportgenerator -reports:$TestFiles -targetdir:$OutputDirectory -reporttypes:Html | Out-Null

Write-Host "Opening file results in browser..."
Invoke-Expression "$($OutputDirectory)/index.html"

Write-Host "Finished"

