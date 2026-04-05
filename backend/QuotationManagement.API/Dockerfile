FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY QuotationManagement.API/QuotationManagement.API.csproj QuotationManagement.API/
RUN dotnet restore QuotationManagement.API/QuotationManagement.API.csproj

COPY . .
RUN dotnet publish QuotationManagement.API/QuotationManagement.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuotationManagement.API.dll"]
