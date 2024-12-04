# Fitnes Članstvo

## Opis
Spletna aplikacija za upravljanje članstva in rezervacij v fitnes centru.

Armen Hodža 63230105
Jakob Vuga 63230361

## Zagon

1. **Kloniraj repozitorij:**
  git clone https://github.com/HodzaArmen/FitnesClanstvo.git

2. **Pull preden začneš delat:**
   git pull origin main
   
3. **Dodaj spremembe na githubu:**
  git add .
  git commit -m "Opis sprememb"
  git push

4. **zagon MS SQL Server docker containerja, Windows**
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-CU15-GDR1-ubuntu-22.04