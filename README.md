# Abelowy Thue Online
Projekt 6c.
Autorzy: Jakub Michalak, Piotr Kryczka, Adam Ryl

# Środowisko
Program został napisany w środowisku .NET 7.0. Aby utworzyć plik wykonywalny umożliwiający rozpoczęcie gry, należy wywołać skrypt _./make-exe.bat_.

# Uruchomienie programu
Aby uruchomić aplikację, należy otworzyć plik **AbelThueOnline.exe**.

Program poprosi o podanie liczby znaków alfabetu, na którym ma się opierać gra. Po wpisaniu wartości z zakresu 1-24 i zatwierdzeniu klawiszem Enter, użytkownik zostanie poproszony o wybór długości słowa, przy której gra ma się zakończyć. Po wpisaniu wartości z zakresu 1-100 i zatwierdzeniu klawiszem Enter, należy wybrać typ pierwszego gracza.

Do wyboru są następujące trzy typy graczy:
- _user_,
- _mcts_,
- _random_.

W przypadku wyboru _mcts_, dostępne są trzy strategie:
- UCB,
- UCBMixMax,
- UCBTuned.

Dodatkowo przy wyborze _mcts_ należy w dwóch kolejnych krokach wybrać liczbę iteracji (dowolna wartość większa od 0), na których ma działać algorytm oraz maksymalny czas wykonania algorytmu w sekundach. Jeżeli zostanie wybrane 0 sekund, zawsze wykonają się wszystkie iteracje.

Po wyborze typu pierwszego gracza następuje wybór typu drugiego gracza. Cały proces jest analogiczny.

Jeżeli zostały wybrane jedynie typy _random_ lub _mcts_, zachodzi symulacja rozgrywki, a kolejne stany gry są drukowane w konsoli.

Załóżmy, że którykolwiek gracz jest typu  _user_.
- W przypadku gdy jest to pierwszy gracz, na konsoli zostanie wyświetlony znak |, którym można poruszać strzałkami po słowie (na początku słowo jest puste, więc poruszanie jest niemożliwy). Po wyborze odpowiedniej pozycji do wstawienia litery przez drugiego użytkownika należy zatwierdzić ją naciskając klawisz Enter. Następnie w drugiej linijce zostaje wyświetlona litera wybrana przez drugiego gracza. W trzeciej linijce znowu pojawia się znak | z literką z drugiej linijki. Po wyborze strzałkami pozycji dla znaku |, należy zatwierdzić pozycję naciskając Enter i powtarzać czynność wyboru pozycji do wstawienia litery aż do zakończenia rozgrywki.
- W przypadku gdy jest to drugi gracz, wyświetlony na konsoli znak | oznacza miejsce, które zostało wybrane przez pierwszego gracza. Znakiem _ zostało oznaczone miejsce, w które należy wpisać literę z alfabetu (jest to alfabet a, b, c, ...). Po wpisaniu litery znak _ zostanie zastąpiony literą. Wybór należy zatwierdzić klawiszem Enter. Następnie wyświetla się nowe słowo, które składa się z wybranej przez drugiego gracza litery i znaku |. Analogicznie należy powtórzyć kroki zastąpienia _ znakiem z alfabetu i zatwierdzenia klawiszem Enter aż do zakończenia rozgrywki. Jeżeli w słowie zostanie wykryta repetycja abelowa, będzie ona wyróżniona wielkimi literami - wtedy wykonanie ruchu jest niemożliwe i trzeba wybrać inną literę.
