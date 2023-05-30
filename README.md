# Środowisko
Program został napisany w środowisku .NET 7.0. Aby utworzyć plik wykonywalny, należy wywołać skrypt ./make-exe.bat.

# Uruchomienie programu
Aby uruchomić aplikację, należy otworzyć plik AbelThueOnline.exe.

Program poprosi o podanie liczby znaków alfabetu, na którym ma opierać się gra. Po wpisaniu wartości z zakresu 1-24 i zatwierdzeniu klawiszem Enter, użytkownik zostanie poproszony o wybór długości słowa, przy której gra ma się zakończyć. Po wpisaniu wartości z zakresu 1-100 i zatwierdzeniu klawiszem Enter, należy wybrać typ pierwszego gracza.

Do wyboru jest następujących trzech typów graczy:
- user,
- mcts,
- random.

W przypadku wyboru mcts, dostępne są do wyboru 3 strategie:
- UCB,
- UCBMixMax,
- UCBTuned.

Dodatkowo przy wyborze mcts należy w dwóch kolejnych krokach wybrać liczbę iteracji (dowolna wartość większa od 0), na których ma działać algorytm oraz maksymalny czas wykonania w sekundach. Jeżeli zostanie wybrane 0 sekund, to zawsze wykonają się wszystkie iteracje.

Po wyborze typu pierwszego gracza następuje wybór typu drugiego gracza. Proces jest analogiczny jak przy wyborze pierwszego gracza.

Jeżeli zostały wybrane jedynie typy random lub mcts, to zachodzi symulacja rozgrywki i kolejne stany gry są drukowane kolejno w konsoli.

Jeżeli natomiast został wybrany typ user to:
- Jeżeli został wybrany jako pierwszy gracz, to zostaje wyświetlony znak |, którym można poruszać strzałkami po słowie (na początku słowo jest puste, więc nie da się poruszać znakiem |). Po wyborze odpowiedniej pozycji do wstawienia litery przez drugiego użytkownika, należy zatwierdzić ją naciskając klawisz Enter. Następnie w drugiej linijce zostaje wyświetlona litera wybrana przez drugiego gracza. W trzeciej linijce znowu pojawia się znak | z literką z drugiej linijki. Po wyborze strzałkami pozycji dla znaku |, należy zatwierdzić pozycję naciskając Enter i powtarzać czynność wyboru pozycji do wstawienia litery, aż do zakończenia rozgrywki.
- W przypadku, gdy userem jest drugi gracz, to wyświetlony jest znak |, oznaczający miejsce, które zostało wybrane przez pierwszego gracza. Znakiem _ zostało oznaczone miejsce, w które należy wpisać literę z alfabetu (jest to alfabet a, b, c, ...). Po wpisaniu litery znak _ zostanie zastąpiony literą. Wybór należy zatwierdzić przyciskiem Enter. W przypadku gdy następuje repetycja, repetycja zostaje wyróżniona wielkimi literami. Następnie wyświetla się nowe słowo, składające się z wybranej przez nas litery i znaku |, analogicznie jak poprzednio zastępujemy _ znakiem z alfabetu i zatwierdzamy znakiem Enter aż do zakończenia gry.