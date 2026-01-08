#!/bin/bash

# Path to the .NET SDK compiler (csc for .NET Framework, dotnet for .NET Core/.NET 5+)
# Adjust if needed
COMPILER="dotnet"

# Output executable name
OUTPUT="ProjectManager.exe"

# Find all .cs files recursively (adjust path if your sources are in a subfolder like src/)
CS_FILES=$(find . -name "*.cs" | tr '\n' ' ')

echo "Compiling all C# files..."
$COMPILER build <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework> <!-- Change to net6.0, net7.0, etc. if needed -->
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
EOF

if [ $? -eq 0 ]; then
    echo "Compilation successful!"
    echo "Launching Controller..."
    ./bin/Debug/net10.0/ProjectManager   # Adjust path/framework if different
else
    echo "Compilation failed."
    exit 1
fi
