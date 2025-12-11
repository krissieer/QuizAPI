FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY Quiz.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish "Quiz.csproj" -c Release -o /publish --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Quiz.dll"]