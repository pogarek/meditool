# Meditool - znajdź termin w Medicover i dowiedz się o tym , jak inny pacjent go zwolni

## Instalacja
1. Pobierz i zanstaluj https://dotnet.microsoft.com/download dla swojej platformy (Windows , Linux, macOS)
2. Pobierz repozytorium (.zip lub przy użyciu git)
3. Wejdź do katalogu z wypakowanym repozytorium i wpisz:
```
dotnet restore
```

## Używanie
1. Zerknij na przykładowe pliki i zrób plik konfiguracyjny na swój użytek
2. Plik dla konsultacji szuka internisty (do celów innych niż grypa/przezięnienia - do tego jest formatka PFM), a dla badania szuka USG jamy brzusznej. Pliki przykładowe PFM szukają szczepienia oraz MedicoverExpress.  Po inne ID danej wizyty (oraz id lekarzy, klinik itp) zerking do  Developer tools w przeglądarce lub w Fiddler'ze. 
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
    "AfterHour" : 0,
    "ExaminationSearchData": {
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
Wydaje mi się, iż wszystko jest jasne :-) Zwróc uwagę czy plik konfiguracyjny zawiera sekcję ***ExamindationSearchData*** dla badań,  ***ConsultationSearchData*** dla wizyt u lekarza lub ***PfmSearchData*** dla wizyt umawianych z innej formatki, jak, na przykład, szczepienia. 

Uwagi:
* UsePushOver można ustawić na False i powiadomienia nie będą używane . Zalecam to na testy aplikacji i włączenie powiadomień, jeśli Ci się spodoba
* DoNotSendPushForSlotsAboveDays to okres dni , począwszy od searchSince, powyżej których apka uznaje iż nie trzeba informowac o wolnym terminie w formie powiadomienia
* Plik *doctors.json.db* to zebrane i zapisane informacje o lekarzach z witryny Medicover. Ta lista może zostać odświeżona, w każdej chwili, przy użyciu https://github.com/pogarek/MediCrank 

W razie problemów skorzystaj z zakładki *Issues* na tej stronie . Ale to wymaga założenia konta na GitHub'ie. 

Podziękowania dla :
* https://github.com/apqlzm/medihunter - za ładne opisanie logiki komunikacji http z MOL . Nie musiałem tego sam badać
* https://github.com/consi/medisnip - za pomysł wykorzystania https://pushover.net/ . Nie miałem pojęcia że taki serwis do powiadomień istnieje. 

Powodzenia w polowaniu!
