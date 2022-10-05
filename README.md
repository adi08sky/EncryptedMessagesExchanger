# EncryptedMessagesExchanger
Projekt aplikacji wymieniających zaszyfrowane wiadomości.

![image](https://user-images.githubusercontent.com/46719355/193958140-60e04901-8686-4174-b944-41b88d07898a.png)

A - BackendConsoleApp, jest aplikacją konsolową.

B - BackendWebApi, jest aplikacją napisaną z wykorzystaniem ASP.NET Core.

Baza danych to MSSQL.

Założenia:
1. A umieszcza w bazie danych zaszyfrowaną wiadomość.
2. A wysyła do B wiadomość zawierającą klucz, pod endpoint api wystawiony przez B.
3. B pobiera zaszyfrowaną wiadomość z bazy danych, a następnie odszyfrowuje ją za pomocą klucza otrzymanego od A.
4. B w odpowiedzi zwraca status „I’m a teapot” wraz z odszyfrowaną wiadomością.
5. A wyświetla na konsoli wiadomość zaszyfrowaną, klucz, oraz wiadomość odszyfrowana.
6. Cykl powtarza się co 15 sekund – za każdym razem wiadomość i klucz do odszyfrowania są inne.
7. Baza danych powstaje metodą code first wraz z projektem aplikacji B.
8. Aplikacja B do kontaktu z bazą danych wykorzystuje entity framework.
9. Aplikacja A do kontaktu z bazą danych wykorzystuje czystego SQL.
10. W aplikacji B do kontrolera wystawiającego endpoint, został wstrzyknięty (dependency injection) obiekt posiadający metodę która przyjmuje 2 parametry, wiadomość oraz klucz, a zwracająca odszyfrowaną wiadomość.
11. Jednocześnie może działać N aplikacji typu A.

 
