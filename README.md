# TPI Element Picker

Nástroj pro mapování prvků webové aplikace TPI a generování skriptů GTO pro Oracle.

Po kliknutí na prvek ve stránce se prvku přiřadí **typ prvku TPI** (lineedit, combobox,
tabulka, sloupec tabulky, záložka, menu, button, connection…). Podle typu aplikace nabídne
jen ty vlastnosti `in_gattrib_overload_name`, které k němu podle `cis_gto.docx` patří,
a z vyplněných hodnot vygeneruje **statické GTO** (`CREATE_GATTRIB_OVERLOAD`) i **dynamické
GTO** (makro `@GATTRIB_OVERLOAD`).

Rozhraní je ve stylu Visual Studia – dokovatelná okna, záložky, auto-hide a **plovoucí okna
vytažitelná úplně mimo hlavní okno** (na druhý monitor).

## Řešení

```
TpiElementPicker.sln
└── src
    ├── TpiGto                 … knihovna: číselník typů, odvození element_path, generátor
    │   ├── Model              … základní struktura (TpiElementType, GtoPropertyDefinition…)
    │   ├── Types              … jeden soubor = jeden typ prvku  ← sem se přidávají nové typy
    │   ├── Registry           … skládání číselníku ze zdrojů (kód + JSON)
    │   ├── Matching           … návrh typu podle kliknutého prvku
    │   ├── Naming             … konvence názvů (tabulka = vše za prefixem LIST)
    │   ├── Paths              … odvození in_ref_element_path
    │   ├── Mapping            … model mapování + JSON export
    │   └── Scripting          … generátor statického a dynamického GTO
    └── TpiElementPicker       … WinForms aplikace (.NET 8, WebView2, DockPanel Suite)
        ├── Workspace          … PickerWorkspace = sdílený stav + události, IBrowserHost
        ├── Forms
        │   ├── MainForm.cs    … menu, panel nástrojů, stavový řádek, dokovací plocha
        │   └── Docking        … jednotlivá dokovatelná okna
        ├── Models             … DOM uzel a zprávy z picker.js
        ├── Services           … nastavení, profily připojení (heslo přes DPAPI)
        ├── Scripts/picker.js  … skript vkládaný do stránky
        └── Data               … elementTypes.custom.json (rozšíření číselníku)
```

Knihovna `TpiGto` je čistý .NET 8 bez závislosti na WinForms ani WebView2 — dá se použít
i z generátoru skriptů (krok 2), z konzole nebo z testů.

## Okna a dokování

| Okno | Role | Výchozí pozice |
|---|---|---|
| Webová aplikace | WebView2 + picker.js | dokument |
| Skript GTO | vygenerovaný skript, kopírování a uložení | dokument (záložka vedle prohlížeče) |
| DOM strom | strom stránky s filtrem | vpravo |
| Prvek | atributy, návrhy `in_ref_element_path` | vpravo dole |
| Typ a vlastnosti | výběr typu + mřížka vlastností GTO | vpravo dole (záložka) |
| Mapování | seznam namapovaných prvků | dole |
| Kontroly | výsledky validace (obdoba Error Listu) | dole (záložka) |
| PageFlow | názvy page flow zachycené v síťové komunikaci | dole (záložka) |

S okny se pracuje jako ve Visual Studiu: tažením za záhlaví se přesouvají podle naváděcích
šipek, dvojklik na záhlaví je vytrhne jako plovoucí okno, špendlík je schová do auto-hide
pruhu, křížek zavře (znovu se otevřou v menu *Zobrazit*).

* **Rozložení se ukládá** do `%APPDATA%\TpiElementPicker\layout.xml` při zavření aplikace,
  ručně přes *Zobrazit → Uložit rozložení oken*.
* *Zobrazit → Obnovit výchozí rozložení* vrátí původní stav.
* *Zobrazit → Motiv* přepíná motiv VS2015 modrý / světlý / tmavý.

Okna spolu nekomunikují přímo — sdílejí `PickerWorkspace` a jeho události
(`ElementPicked`, `TreeLoaded`, `MappingChanged`, `IssuesChanged`…). Proto je jedno,
kde okno zrovna je, jestli je plovoucí nebo zavřené.

## Postup práce

1. **Profil a přihlášení** – *Nástroje → Profily přihlášení* (URL + ruční / HTTP Basic /
   vyplnění formuláře podle CSS selektorů). Heslo se ukládá zašifrované DPAPI.
2. **Načíst** stránku, pak **Načíst DOM (F4)** pro naplnění stromu.
3. **Vybrat prvek (F2)** → klikni na prvek ve stránce. Akce stránky se potlačí, prvek se
   zvýrazní, vybere se ve stromu a v okně *Prvek* se ukážou jeho atributy, CSS a XPath.
4. Aplikace navrhne `in_ref_element_path` a typ prvku; typ jde přepsat.
5. V okně *Typ a vlastnosti* zaškrtni vlastnosti, vyplň hodnoty, zvol režim
   (statické / dynamické). **F5** uloží prvek do mapování.
6. *Generovat* (Ctrl+G) vytvoří skript; nálezy validace jsou v okně *Kontroly*,
   dvojklik na řádek otevře dotčený prvek k úpravě.
7. *Soubor → Uložit mapování* uloží JSON — vstup pro generátor skriptů (krok 2).

Klávesy: **F2** výběr prvku, **F4** načíst DOM, **F5** uložit prvek do mapování,
**Ctrl+G** generovat, **Ctrl+O / Ctrl+S** mapování.

## Pravidla převzatá ze školení (přepis Jakub Kočí)

Generátor a kontroly z něj přímo vycházejí:

* **Dvě obrazovky, dvě skladby cesty.** *Seznam* je obraz celé databázové tabulky,
  *detail* (formulář) obraz jednoho řádku. Typ obrazovky se přepíná v panelu nástrojů
  (`Obrazovka: ? / Seznam / Detail`) a ukládá se do mapování.
* **Na seznamu se tabulka jmenuje vždy `ObjectList`.** Aplikace ho u typů TABLE
  a TABLE_COLUMN rovnou předvyplní; pokud je zadáno něco jiného, kontroly to označí.
  V detailu se child tabulky jmenují `child_<TABULKA>`.
* **Změna celé tabulky** (šířka, filtr, řazení) → cesta `ObjectList`.
  **Změna sloupce** (skrytí, popisek v hlavičce, pořadí) → cesta musí obsahovat
  i sloupec, a to jeho **fyzický název v databázi**, ne popisek z hlavičky.
  Typy s `RequiresColumn` na chybějící sloupec upozorní chybou.
* **Název elementu je unikátní jen v rámci page flow**, ne v celé aplikaci – proto je
  mapování vždy svázané s jedním `in_pf_name`. Překlep v názvu page flow je častá příčina
  toho, že se změna tiše neprojeví, proto se název kontroluje (mezery, malá písmena).
* **Kategorie vlastností** podle prefixu: `M_Pf_Dt_*` mění data / zobrazený text,
  `M_Pf_*` vzhled, `M_Pf_Element.*` je obecné (typicky `Visible` = 0/1, nejpoužívanější
  gattrib), `M_Pf_Connection.*` napojení událostí. Mřížka vlastností kategorii zobrazuje.
* **Klíč GTO** je trojice `in_pf_name` + název elementu + název gattribu. Pokud záznam
  existuje, přepíše se, jinak se založí — hodnotu tedy lze přepisovat donekonečna.
  Kontroly hlásí, když je táž vlastnost na stejném elementu nastavena vícekrát.
* **Zrušení změny** se nedělá vrácením původní hodnoty, ale stejným GTO s `in_zruseno = 1`
  (sloupec *Zrušit* v mřížce) — zrušený záznam se při spuštění vůbec nenahrává.
* **Statické GTO se spouští první, dynamické z MWF až po něm a hodnotu přepíše.**
  Když je táž vlastnost nastavena oběma způsoby, kontroly na to upozorní.
* **Špatně zvolený gattrib nehlásí chybu** — na obrazovce se prostě nic nestane.
  Proto se vlastnosti nabízejí jen z číselníku podle typu a neznámá vlastnost se hlásí.
* **DBMS Output** je potřeba mít v developeru zapnutý, jinak nepoznáš, že skript
  skončil OK (nepovolená hodnota projde bez povšimnutí). Generovaný skript s tím
  začíná v komentáři.
* **Prostředí** aplikace rozlišuje barevným pruhem: dev hnědý, preprod růžový,
  produkce modrý. Profil připojení má proto položku *Prostředí* a stavový řádek
  barevný štítek.
* **Reset cache page flow.** Page flow si drží cache zobrazení atributů, takže se změna
  nemusí projevit. Do statického skriptu se proto přidává reset cache
  (*Nastavení → Skript*); dokud není doplněno volání procedury, vloží se do skriptu
  jen připomínka v komentáři. Cenou je pomalejší první načtení obrazovky.
* **Kde zkoušet:** nejdřív na devu nad objednávkou / fakturou (menu Test), teprve pak
  nad číselníky – neexistující gattriby tam umí nadělat nepořádek.

## Co přidala dokumentace Gattrib_Overload.docx a katalog maker

* **Číselník vlastností je teď kompletní** – 174 vlastností v typech plus společné
  `M_Pf_Element.*` a `M_Pf_Widget.*`, u každé je ID z `m_cis_gattrib_overload`
  a informace, zda je doložená seznamem GMSG, jen v `cis_gto.docx`, nebo v katalogu maker.
* **Typy overloadu.** Iniciální GTO může být `DEFAULT_OVERLOAD`, `ROLE_OVERLOAD`
  (podle role, vyžaduje `in_ref_role` = m_role.id), `USER_OVERLOAD` (podle uživatele,
  `in_ref_login` = tpi_uzivatel.id) nebo `SECURITY_OVERLOAD` (běží i při každé iteraci,
  takže přebije i runtime overload). Nastavuje se u každé vlastnosti v mřížce.
* **Runtime GTO nad daty** – `@GATTRIB_OVERLOAD_DATA` se provede až po namapování
  business dat do GMSG, takže hodnota může na datech záviset (obarvit pole podle obsahu).
  V mřížce je to třetí režim vedle statického a runtime.
* **Vytvoření nového elementu.** Overloadem lze založit nový element typu `layout`
  nebo `popup` (parametr `in_element_typ`); pak je nutné ho zařadit pod existující
  element vlastností `M_Pf_Element.M_Pf_Element_Name`. Nastavuje se v okně *Prvek*
  a kontroly hlídají obojí.
* **Skladba cesty** je potvrzená: název elementu `M_PF_ELEMENT.ELEMENT_NAME`, k tomu
  u sloupce `M_PF_DT_TABLE_COLUMN.NAME`, u položky menu `M_PF_DT_MENU_ITEM.NAME`,
  u záložky `M_PF_DT_TAB_ITEM.NAME` (např. `RelTabs.POLOZKY_FA`) a u eventu jeho typ.
  Typy, které podřízený prvek vyžadují, na jeho chybějící vyplnění upozorní chybou.
* **Vstupní body PageFlow** (`M_Pf.Mwf_Name_Init`, `M_Pf.M_Mwf_Name_Reinit`,
  `M_Pf.Mwf_Name_Leave`) se zadávají na element `Window` – jsou v typu WINDOW.
  Init MWF nelze konfigurovat v runtime, což kontroly hlídají.
* **Eventy** mají kromě `M_Pf_Connection.M_Wf_Name` i `M_Mwf_Name_Before`
  a `M_Mwf_Name_After`.
* **Nové typy prvků:** POPUP, TREE, TREE_NODE, LIST, LIST_ITEM, MAP, COMBOBOX_VALUE,
  TABLE_SELECTED_ROW.

## in_ref_element_path

Ve frameworku TPI je element_path **název elementu**, ne CSS selektor —
např. `ContainerL_PTS_VRSTVA_DAT_FILTR_PTS_CIS_TYP_DAT_KOD_Field`, `child_PZSV_KOL_LOZE`,
`MainMenu.SAVE_AND_CLOSE`, `ObjectList.DODAVATEL_ID`, `child_POLOZKY_FA.CAS`.

Podle školení nese název elementu v HTML atribut **`name`** — `id` je generované stránkou
a pro GTO se nepoužívá. Odvozování proto začíná u `name`:

* pořadí prohledávaných atributů je v *Nastavení → element_path → Zdrojové atributy*
  (výchozí: `name`, `data-element-path`, `data-element`, `data-tpi-element`, `data-name`, `id`),
* hledá se i u rodičů (nastavitelná hloubka),
* hodnotu lze očistit předponou/příponou nebo regexem se skupinou `(?<path>…)`,
* v okně *Prvek* jde jiný atribut kdykoli označit a tlačítkem „Zapamatovat atribut jako
  zdroj“ povýšit na primární — kdyby se konvence lišila obrazovku od obrazovky.

Sloupec (`.DODAVATEL_ID`) nebo event (`.blur`) se přidává polem *Sloupec / event*,
u typu CONNECTION nabídkou *Event*.

## Zjištění názvu PageFlow

Název page flow se podle školení hledá v konzoli prohlížeče v síťových požadavcích.
Aplikace to dělá sama: sleduje požadavky i odpovědi stránky a názvy, které najde,
sbírá do okna **PageFlow** (s odvozeným typem obrazovky a fyzickým názvem tabulky).
Dvojklik na řádek název nastaví do panelu nástrojů a rovnou podle něj nastaví i typ
obrazovky. Vzory hledání jsou regulární výrazy v *Nastavení → PageFlow*.

Fyzický název tabulky je v názvu page flow **všechno za prefixem `LIST`** —
`LIST_PZSV_DOPRAVNI_URCENI` → tabulka `PZSV_DOPRAVNI_URCENI`. V té tabulce se pak hledají
technické názvy sloupců (popisek v hlavičce jim odpovídat nemusí).

## Přidání nového typu prvku

**Kódem** (doporučeno pro trvalé typy) — nová třída v `src/TpiGto/Types`:

```csharp
using TpiGto.Model;

namespace TpiGto.Types;

public sealed class MapElementType : TpiElementType
{
    public override string Code => "MAP";
    public override string Name => "Mapa";
    public override int SortOrder => 160;

    public override ElementMatchRule MatchRule => new()
    {
        ClassHints = new[] { "map", "leaflet" }
    };

    protected override IEnumerable<GtoPropertyDefinition> DefineProperties()
    {
        yield return new GtoPropertyDefinition(
            "M_Pf_Map.Visible", "Zobrazení mapy 0/1", GtoValueKind.Bool01, example: "1");
    }
}
```

Nikde se neregistruje — `BuiltInElementTypeProvider` ji najde reflexí.

**Bez rekompilace** — `Data\elementTypes.custom.json` vedle .exe. Typ se stejným `code`
přepíše vestavěný typ, takže jde i opravit nebo doplnit vlastnost. Po úpravě stačí
*Nástroje → Znovu načíst číselník typů*.

**Společné vlastnosti** `M_Pf_Element.*` přidává registr ke každému typu
(`CommonGtoProperties`); typ je může vypnout přes `IncludesCommonProperties`.

**Nový výstupní formát** — implementuj `IGtoScriptWriter` a zaregistruj ho přes
`GtoScriptGenerator.RegisterWriter`.

**Nové dokovatelné okno** — poděď `ToolWindowBase`, v `BuildUi()` poskládej obsah,
v `Subscribe()` se napoj na události workspace, a v `MainForm` ho přidej do
`AllWindows()`, `DeserializeContent()` a menu *Zobrazit*.

## Zdroje číselníku

Typy a vlastnosti vycházejí z dokumentů projektu: `cis_gto.docx` (číselník vlastností),
`staticke_GTO.docx`, `dynamicke_GTO.docx`, `frameworkTPI.docx` (MWF, bloky, BLUR),
`makra.docx` (např. `M_Pf_Tab.Active_Item`, right-click), `Zaskoleni_TPI_PZSV.docx`.

Zvláštnosti převzaté z číselníku doslova: `M_Pf_Dt_Assigner.Tooltyp` a
`M_Pf_Df_Table_Value.Html_Style` (prefix `Df`, nikoli `Dt`).

## Sestavení

* Visual Studio 2022 / 2026, .NET 8 SDK, Windows.
* `dotnet build TpiElementPicker.sln` nebo otevřít solution a spustit `TpiElementPicker`.
* Na počítači musí být **WebView2 Runtime (Evergreen)** — na Win11 je součástí systému.
* Balíčky: `Microsoft.Web.WebView2`, `System.Security.Cryptography.ProtectedData`,
  `DockPanelSuite`, `DockPanelSuite.ThemeVS2015`.
  Pokud by verze 3.1.0 nešla obnovit, zvyš ji na poslední dostupnou — API zůstává stejné.
  Bez motivů VS2015 lze balíček `DockPanelSuite.ThemeVS2015` vypustit; pak z `MainForm`
  smaž volání `ApplyTheme` (zůstane výchozí motiv DockPanel Suite).

Kód prošel syntaktickou kontrolou (parser C#), ale **nebyl zkompilován** — v prostředí, kde
vznikl, není .NET SDK ani Windows a stahování balíčků je blokované. První build prověř ve
Visual Studiu.

## Data aplikace

```
%APPDATA%\TpiElementPicker\settings.json   … nastavení (element_path, hlavička skriptů, motiv)
%APPDATA%\TpiElementPicker\layout.xml      … rozložení dokovaných oken
%APPDATA%\TpiElementPicker\profiles.json   … profily připojení (heslo DPAPI)
%APPDATA%\TpiElementPicker\WebView2\       … profil prohlížeče (drží přihlášení)
```

## Známá omezení

* Strom se staví jen z hlavního dokumentu — prvky v `iframe` zatím nejsou pokryté
  (řešení: `CoreWebView2.FrameCreated` + injektáž skriptu do rámců).
* U SPA je strom snímek stavu; po přestavbě obrazovky je potřeba *Načíst DOM* znovu.
* U generovaných id (React/Angular) jsou selektory nestabilní — proto se element_path
  odvozuje přednostně z `data-*` / `name`.
* Okna se skládají kódem (ne v návrháři) — dokovací kontejnery se v návrháři WinForms
  stejně editují špatně; layout každého okna je v jeho `BuildUi()`.
