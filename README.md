# Kredi Pusulası (Finansal Asistan)

Kullanıcının serbest metinle anlattığı finansal durumunu (gelir, gider, istenen kredi tutarı/vadesi) doğal dil işleme ile ayrıştırıp, BDDK kurallarına göre uygunluk değerlendirmesi yapan ve uygun banka profilleriyle eşleştiren uçtan uca bir web uygulaması.

HangiKredi bünyesindeki staj sürecinde geliştirilmiştir.

## Proje Yapısı
.
├── FinansalAsistanApi/ # Backend (.NET 9 Web API)
└── finansal-asistan-frontend/ # Frontend (Next.js)


## Teknoloji Stack'i

### Backend
- **Dil / Framework:** C# / .NET 9, ASP.NET Core Web API
- **Kimlik Doğrulama:** JWT (access + refresh token), httpOnly + Secure cookie üzerinden taşınıyor, token rotation ile
- **Veritabanı:** MongoDB
- **Doğal Dil İşleme:** Mistral AI (chat completion API) — serbest metinden finansal veri çıkarımı ve sohbet
- **Fallback:** Mistral API'ye erişilemediğinde devreye giren yerel ML.NET modeli (token bazlı çok sınıflı sınıflandırma)
- **Dokümantasyon:** Swagger / OpenAPI

### Frontend
- **Framework:** Next.js (App Router), TypeScript
- **State Yönetimi:** React Context API (`AuthContext`, `ProfileContext`)
- **Stil:** Tailwind CSS

## Öne Çıkan Özellikler

- **Doğal dil ile veri girişi:** "Aylık gelirim 45000 TL, 100000 TL kredi istiyorum, 24 ay vadeli" gibi serbest metinlerden yapılandırılmış veri çıkarımı; "45k", "200 binlik" gibi ifadeleri doğru şekilde sayıya çevirme
- **Konuşma hafızası:** Kullanıcı bilgiyi parça parça verse bile (birden fazla mesaja yayılsa da) profil doğru şekilde birleştiriliyor
- **Güvenlik filtreleri:** Prompt injection, kimlik sorgulama ve hassas veri (TC kimlik/IBAN) üretme girişimlerine karşı çok katmanlı koruma
- **BDDK kurallarına dayalı uygunluk motoru:** Kredi türüne göre (ihtiyaç/taşıt/konut) gerçekçi tutar-vade kısıtlamaları, borç/gelir oranına göre 5 seviyeli risk değerlendirmesi
- **Senaryo/stres testi:** "Gelirim %20 azalırsa ne olur" gibi varsayımsal durumları karşılaştırmalı olarak test edebilme
- **Kimlik doğrulama:** JWT + refresh token ile güvenli oturum yönetimi, httpOnly cookie tabanlı (XSS'e karşı dayanıklı), sayfa yenilendiğinde otomatik oturum geri yükleme
- **Dayanıklı mimari:** LLM sağlayıcısı (Mistral) erişilemez olursa otomatik olarak yerel ML.NET modeline düşen fallback mekanizması

## Kurulum

### Backend

```bash
cd FinansalAsistanApi
dotnet restore

# Gerekli user-secrets'ları ekle
dotnet user-secrets set "Mistral:ApiKey" "<mistral-api-key>"
dotnet user-secrets set "Jwt:Secret" "<en-az-32-karakterlik-rastgele-anahtar>"
dotnet user-secrets set "Jwt:RefreshSecret" "<farklı-en-az-32-karakterlik-anahtar>"
dotnet user-secrets set "Jwt:ExpireMinutes" "15"
dotnet user-secrets set "Jwt:RefreshExpireDays" "30"

# MongoDB'nin yerelde çalıştığından emin ol (appsettings.json: mongodb://localhost:27017)
dotnet run
```

API varsayılan olarak `http://localhost:5142` üzerinde ayağa kalkar, Swagger arayüzü `/swagger` altında erişilebilir.

### Frontend

```bash
cd finansal-asistan-frontend
npm install

# .env.local dosyası oluştur
echo "NEXT_PUBLIC_API_BASE_URL=http://localhost:5142" > .env.local

npm run dev
```

Uygulama `http://localhost:3000` üzerinde çalışır.

## Mimari Notlar

- Access token ve refresh token, XSS'e karşı korunmak amacıyla JavaScript'in erişemediği httpOnly cookie'lerde tutulur; backend, token'ı standart `Authorization: Bearer` header'ı yerine cookie'den okuyacak şekilde özelleştirilmiştir.
- Kredi uygunluk kuralları (`IhtiyacKrediRules`, `TasitKrediRules`) iş mantığından ayrık, statik sınıflarda tutulur.
- Banka profil verileri (`BankProfileSeeder`) gerçek bankacılık pratiklerine yakın, kredi türüne göre farklılaşan vade/tutar aralıklarıyla oluşturulmuştur.

## Bilinen Sınırlamalar / Backlog

- Konut kredisi için LTV (loan-to-value) kurallarının eklenmesi
- Refresh token blacklist mekanizması (Redis ile) — şu an token rotation ile kısmi çözülmüş durumda
- Banka profillerine kampanya/promosyon bilgisi eklenmesi