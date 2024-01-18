

## Egy világvége váró szekta vezetője vagy és beüt a szar (mondjuk zombik). feladatod a követőid közül minél többet életben tartani bármi áron. A követőid mentális állapota zuhan ahogy az új világ szörnyűségeit megtapasztalják akár ön és köz veszéjesekké is válnak. az erőforrások szűkösek és élet halál harcot kell vívni értük.

## Alap mechanikák:
	- top down nézet RTS stílusú irányítással
	- karakterek egyéni felszerelése
	- karakterek igényei (víz, kaja, mentális állapot)
	- minden este random megtámadhatják a bázist és meg kell védeni.
	- napi 1 portyát lehet lejátszani ahol felfedezed a mapot ami kis pályákból áll és gyűjthetsz cuccokat.
	- minden nap végén van egy gyűlés ahol döntéseket lehet hozni amik befolyásolják a közösség túlélését.
		ezek a döntések lehetnek:
		- valaki száműzése
		- ima/beszéd (a fanatikus és az önzetlen emberek között eltolja a balanceot)
		- random eventek kiértékelése
		- talált túlélők sorsa

## A karaktereknek személyiségének 2 végpontja van :
	- radikális / fanatikus
	- önzetlen / fillantróp (mindenkin segíteni akar)


## A gyűléseken hozott döntések hatására kialakul hogy mely irányba viszed el a szektát (tehát valamely fajta ember hátrányban lesz a csoportban)
	- a radikális iránynak sokkal nagyobb a mentális teherbírása viszont nem segít senkinek mindenkit megölnek akit látnak
	- az önzetlen oldalon minden emberveszteség nagy morál mínusszal jár legyen az a csapat tagja vagy random túlélő de cserébe a talált túlélők csatlakozhatnak a "nyájadhoz"

## A csoport hozzáállásának függvényében új lehetőségek nyílnak meg:
	### Radikális vonalon:
	- "csak mi számítunk" - más túlélő táborok kifosztása
	- "A hit majd erőt ad" - karakterek étel és víz fogyasztása csökken debuff nélkül
	- "A cél szentesíti az eszközt" - talált túlélők új sors opciója: ők a vacsora (kannibalizmus)
	- "Csak a legerősebbek élik túl" - új esti döntés: a kolónia tagjai közül is fel lehet áldozni valakit kajának

	### Őnzetlen vonalon:
	- "Egységben az erő" - a csatlakozott túlélők felajánlják a cuccaikat a kolóniának
	- "öld a holtat mentsd az élőt" - idegen túlélők kisebb eséjjel ellenségesek és a karakterek nem veszítenek morált zombik öléséért
	- "egy szebb holnapért!" - a karakterek  kapnak egy last stand abilityt: ha a hp 25% alá esik + 50% dmg;
	- "új világrend" - az ellenséges emberek megöléséért sem jár morál vesztés

## A karaktereknek alapból nő egy kicsit a morálja bizonyos tevékenységektől:
	- evés
	- pihenéssel töltött nap
	- morálvesztés nélküli nap

## A játéknak vége ha minden embered meghal. vagyis egyedül maradsz. nem sikerült megmenteni a nyájadat



A UI fekete-szürkeárnyalatok-vérvörös skálából dolgozik, a hangulat nyomasztó, sötét, reményvesztett, kevés és tompa színű alapból.

a játékos választásától függően változik a játék hangulata (ha a fanatikus irányba megy akkor még piros-szürke fekébb lesz,ha a segítő irányba megy akkor több lesz a szín, de ugyanúgy összességében sötét és nyomasztó lesz)


# pályák, hátterek és prop-ok:
  ## a háttereknek kéne két verzió
	- egy szektásabb (piros-szürke-fekete)
	- meg egy kevésbé szektásabb (kevesebb piros, több más szín, kicsit kevésbé nyomsztó légkör) 
a játék stratégiai része top down, szóval a pályaelemek is azok, viszont azokból csak 1 féle kell, az lehet normális, majd post-processel kiveszem a színeket runtime.
a pálya grid rendszerrel lesz megépítve szóval minden egységnek 1x1 méretűnek kell lenni és lehetőleg olyan mintával hogy végteleníthető legyen önmagával kombinálva (a propok lehetnek nagyobbak mert azoknak nem kell a gridre illeszkednie)
## hátterek:
	- main game screen háttér:
	egy templom ami előtt egy tábortűz fatönkökkel körberakva (nem ég), a terület saras

## MAP egységek (top-down):
	- különböző füves mező min. 4db
	- különböző földes mező min. 4db
	- fatönk
	- ház padló parketta
	- ház padló csempe
 ## propok (top-down):
	- fa láda
	- fa hordó
	- autóroncs
	- szőnyeg
	- asztal
	- szobanövény
	- kanapé
	
## fegyverek (top-down + side view inventory ikon):
	- AK47
	- vascsövekből összetákolt géppisztoly
	- valami vadászpuska
	- egy Glock
	- vascső
	- baseball ütő
	- nagy bozótvágó kés

## UI háttér:
  pixelart sötétszürke háttérpanel sötétebb szürke kerettel (átméretezhetőnek kéne lennie ez lenne az összes UI alapja)
  háttér néküli ikonok:
  pentagramm (vörös)
  egy tál benne két csont (barna-fehér-világos szürke)
  egy tál fűrészpor
  tilos jelzés (várakozni/parkoli tilos ikon)
  kisgyerek kéz kalapácsot fog
  emberek állnak egymás mellet/mögött (hoi4 manpower ikon)
  erőforrás ikon pl.:(fogaskerék + fadeszkák)
  kaja ikon (csülök vagy csirkecomb vagy kenyér)
  égő hátterű emberalak (ember fekete, tűz sárgás narancs)


## emberek:
  kell legalább 5 féle felülnézeti modell (kezek nélkül)
  min 5db oldalnézeti modell, (teljes testes portré szerűen, mint egy inventoryban)
  ugyanezekből egy-egy "profilkép" szerű portré

## particle sytemhez cuccok (ezek sokszorosításából lesz a particle system):
	- vonalszerű esőcsepp (oldalnézet)
	- muzzle flash (nem kell túlbonyolítani 3 db elég) sárgás-narancsos-fehér árnyalat
	- tábortűz (oldalnézet)
	
