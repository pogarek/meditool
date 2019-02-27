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

## Przykładowy plik konfiguracyjny (dla badania)
```
{
    "username" : "mol_username",
    "password": "mol_password",
    "UsePushOver" : true,
    "pushOverUserId" : "take it from pushover.net dashboard",
    "pushOverAppTokenId": "register app on pushover dashboard",
    "DoNotSendPushForSlotsAboveDays" : 4,
    "CheckIntervalMinutes" : 5,
    "ExamindationSearchData": {
        "regionId": 204,
        "bookingTypeId": 1,
        "serviceId": 521,
        "clinicId": -1,
        "languageId": -1,
        "doctorId": -1,
        "searchSince": "2019-02-18T02:00:00.000Z",
        "searchForNextSince": null,
        "periodOfTheDay": 0,
        "isSetBecauseOfPcc": false,
        "isSetBecausePromoteSpecialization": false
    }
}
```
Wydaje mi się, iż wszystko jest jasne :-) Zwróc uwagę czy plik konfiguracyjny zawiera sekcję *ExamindationSearchData* dla badań i *ConsultationSearchData* dla wizyt u lekarza. 
Uwagi:
* UsePushOver można ustawić na False i powiadomienia nie będą używane . Zalecam to na testy aplikacji i włączenie powiadomień, jeśli Ci się spodoba
* DoNotSendPushForSlotsAboveDays to okres dni , począwszy od searchSince, powyżej których apka uznaje iż nie trzeba informowac o wolnym terminie w formie powiadomienia

Niektóre wizyty używają innych formatek na stronie MOl (np. szczepienia). Wsparcie dla takich wizyt jest planowane , muszę tylko znaleźć czas na to. 

Podziękowania dla :
* https://github.com/apqlzm/medihunter - za ładne opisanie logiki komunikacji http z MOL . Nie musiałem tego sam badać
* https://github.com/consi/medisnip - za pomysł wykorzystania https://pushover.net/ . Nie miałem pojęcia że taki serwis do powiadomień istnieje. 

Powodzenia w polowaniu!
