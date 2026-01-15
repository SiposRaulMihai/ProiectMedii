docker start parcauto-sql

# Oprește orice proces anterior

killall dotnet || true

# Șterge datele vechi și aplică migrările pe curat

dotnet ef database drop --force

dotnet ef database update

# Pornește serverul pe portul necesar emulatorului

dotnet run --urls http://0.0.0.0:5002

# Șterge reziduurile de build pentru a evita erorile de cache

rm -rf bin obj

dotnet clean

# Compilează și instalează aplicația (doar pentru Android)

dotnet build -f net9.0-android -t:Install

# Comanda de start pe care ai cerut-o anterior (adb monkey)

adb shell monkey -p com.companyname.parcauto_web_app -c android.intent.category.LAUNCHER 1
