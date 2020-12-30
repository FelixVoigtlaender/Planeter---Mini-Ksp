print(eingabe)
print("")

zeilen_zeichen = "\n"
trenn_zeichen = "|"
zahl_zeichen = "."

#Zeile Splitten
zeilen = eingabe.split(zeilen_zeichen)
print(zeilen)
print("")

#Pärchen
pairs = []
for zeile in zeilen:
    pairsInLine = zeile.split(trenn_zeichen)
    for pair in pairsInLine:
        pairs.append(pair)
print(pairs)
print("")

zahlen =  []
for pair in pairs:
    if "." in pair:
        for zahl in pair.split(zahl_zeichen):
            if(zahl.isdecimal()):
                zahlen.append(zahl)
print(zahlen)
print("")
