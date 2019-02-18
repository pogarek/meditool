## Instalacja
1. Pobierz i zanstaluj https://dotnet.microsoft.com/download dla swojej platformy
2. Pobierz repozytorium (.zip lub przy użyciu git)
3. Wejdź do katalogu z wypakowanym repozytorium i wpisz:
```
dotnet restore
```

## Używanie
1. Zerknij na przykładowe pliki i zrób plik konfiguracyjny na swój użytek
2. Plik dla konsultacji szuka internisty a dla badania szuka USG jamy brzusznej. Po inne ID danej wizyty zobacz w przeglądarce w Developer tools lub w Fiddler'ze
3. uruchom narzedzie :
```
dotnet run <plik_config.json>
```

Podziękowania dla :
* https://github.com/apqlzm/medihunter - za ładne opisanie logiki komunikacji http z MOL . Nie musiałem tego sam badać
* https://github.com/consi/medisnip - za pomysł wykorzystania https://pushover.net/ . Nie miałem pojęcia że taki serwis do powiadomień istnieje. 

Powodzenia w polowaniu!