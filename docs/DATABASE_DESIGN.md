# 🗄️ Veritabanı Tasarımı - Hospital Management System

## 📊 Entity Relationship Diagram (ERD)

```
Patient ||--o{ Appointment : has
Doctor ||--o{ Appointment : provides
Doctor ||--o{ DoctorSchedule : has
Department ||--o{ Doctor : contains
```

---

## 📋 Tablo Detayları

### 🏥 Department (Bölüm/Branş)
| Alan | Tür | Açıklama | Kısıtlar |
|------|-----|----------|----------|
| Id | int | Primary Key | PK, Auto Increment |
| Name | string(100) | Bölüm adı | Not Null, Unique |
| Description | string(500) | Bölüm açıklaması | Nullable |
| IsActive | bool | Aktif durumu | Default: true |
| CreatedDate | DateTime | Oluşturulma tarihi | Default: Now |

**Örnek Veriler:**
- Kardiyoloji, İç Hastalıkları, Göz Hastalıkları, Ortopedi, vb.

---

### 👨‍⚕️ Doctor (Doktor)
| Alan | Tür | Açıklama | Kısıtlar |
|------|-----|----------|----------|
| Id | int | Primary Key | PK, Auto Increment |
| FirstName | string(50) | Ad | Not Null |
| LastName | string(50) | Soyad | Not Null |
| Title | string(20) | Unvan | Not Null (Dr., Prof. Dr., Doç. Dr.) |
| DepartmentId | int | Bölüm ID | FK to Department |
| LicenseNumber | string(20) | Diploma numarası | Unique, Not Null |
| Phone | string(15) | Telefon | Nullable |
| Email | string(100) | E-posta | Unique, Nullable |
| IsActive | bool | Aktif durumu | Default: true |
| CreatedDate | DateTime | Oluşturulma tarihi | Default: Now |

**Computed Properties:**
- FullName: FirstName + " " + LastName
- DisplayName: Title + " " + FullName

---

### 📅 DoctorSchedule (Doktor Çalışma Takvimi)
| Alan | Tür | Açıklama | Kısıtlar |
|------|-----|----------|----------|
| Id | int | Primary Key | PK, Auto Increment |
| DoctorId | int | Doktor ID | FK to Doctor |
| DayOfWeek | int | Haftanın günü | 1-7 (1:Pazartesi, 7:Pazar) |
| StartTime | TimeSpan | Başlangıç saati | Not Null |
| EndTime | TimeSpan | Bitiş saati | Not Null |
| AppointmentDuration | int | Randevu süresi (dk) | Default: 30 |
| IsActive | bool | Aktif durumu | Default: true |

**İş Kuralları:**
- Aynı doktor, aynı günde çakışan saatler olamaz
- EndTime > StartTime olmalı
- AppointmentDuration 15-60 dk arası olmalı

---

### 👤 Patient (Hasta)
| Alan | Tür | Açıklama | Kısıtlar |
|------|-----|----------|----------|
| Id | int | Primary Key | PK, Auto Increment |
| FirstName | string(50) | Ad | Not Null |
| LastName | string(50) | Soyad | Not Null |
| IdentityNumber | string(11) | TC Kimlik No | Unique, Not Null |
| BirthDate | DateTime | Doğum tarihi | Not Null |
| Gender | string(10) | Cinsiyet | Not Null (Erkek/Kadın) |
| Phone | string(15) | Telefon | Not Null |
| Email | string(100) | E-posta | Unique, Nullable |
| Address | string(500) | Adres | Nullable |
| EmergencyContact | string(100) | Acil durum kişisi | Nullable |
| EmergencyPhone | string(15) | Acil durum telefon | Nullable |
| IsActive | bool | Aktif durumu | Default: true |
| CreatedDate | DateTime | Kayıt tarihi | Default: Now |

**Computed Properties:**
- FullName: FirstName + " " + LastName
- Age: Calculated from BirthDate

---

### 📋 Appointment (Randevu)
| Alan | Tür | Açıklama | Kısıtlar |
|------|-----|----------|----------|
| Id | int | Primary Key | PK, Auto Increment |
| PatientId | int | Hasta ID | FK to Patient |
| DoctorId | int | Doktor ID | FK to Doctor |
| AppointmentDate | DateTime | Randevu tarihi | Not Null |
| Duration | int | Süre (dakika) | Default: 30 |
| Status | string(20) | Randevu durumu | Not Null |
| Complaint | string(1000) | Şikayet | Nullable |
| Notes | string(1000) | Notlar | Nullable |
| CreatedDate | DateTime | Oluşturulma tarihi | Default: Now |
| CreatedBy | string(50) | Oluşturan | Default: "System" |
| UpdatedDate | DateTime | Güncellenme tarihi | Nullable |

**Status Enum Değerleri:**
- `Scheduled` (Planlandı)
- `Confirmed` (Onaylandı)
- `InProgress` (Devam Ediyor)
- `Completed` (Tamamlandı)
- `Cancelled` (İptal Edildi)
- `NoShow` (Gelmedi)

**İş Kuralları:**
- Aynı doktor, aynı tarih/saatte birden fazla randevu alamaz
- Geçmiş tarihlere randevu oluşturulamaz
- Randevu saati doktorun çalışma saatleri içinde olmalı
- Patient ve Doctor aktif durumda olmalı

---

## 🔄 İlişkiler (Relationships)

### One-to-Many İlişkiler:
1. **Department → Doctor** (1:N)
   - Bir bölümde birden çok doktor olabilir
   - Bir doktor sadece bir bölümde çalışır

2. **Doctor → DoctorSchedule** (1:N)
   - Bir doktor birden çok çalışma saati tanımlayabilir
   - Her çalışma saati sadece bir doktora aittir

3. **Doctor → Appointment** (1:N)
   - Bir doktor birden çok randevu verebilir
   - Her randevu sadece bir doktora aittir

4. **Patient → Appointment** (1:N)
   - Bir hasta birden çok randevu alabilir
   - Her randevu sadece bir hastaya aittir

---

## 📝 Seed Data Örnekleri

### Departments:
```json
[
  { "Name": "Kardiyoloji", "Description": "Kalp ve damar hastalıkları" },
  { "Name": "İç Hastalıkları", "Description": "Genel dahili tıp" },
  { "Name": "Göz Hastalıkları", "Description": "Göz muayene ve tedavi" }
]
```

### Doctors:
```json
[
  {
    "FirstName": "Ahmet",
    "LastName": "Yılmaz",
    "Title": "Prof. Dr.",
    "DepartmentId": 1,
    "LicenseNumber": "123456789",
    "Phone": "0532-123-4567",
    "Email": "ahmet.yilmaz@hastane.com"
  }
]
```

---

## 🔒 İndeksler (Indexes)

### Performance için önerilen indeksler:
- `Patient.IdentityNumber` (Unique)
- `Patient.Phone`
- `Doctor.LicenseNumber` (Unique)
- `Appointment.AppointmentDate`
- `Appointment.PatientId`
- `Appointment.DoctorId`
- `Appointment.Status`

---

## 📊 Validasyon Kuralları

### Patient Validasyonları:
- TC Kimlik No: 11 haneli, sayısal
- Telefon: +90 formatında
- E-posta: Geçerli format
- Doğum tarihi: Gelecek tarih olamaz

### Appointment Validasyonları:
- Randevu tarihi: Gelecek tarih olmalı
- Çalışma saati kontrolü
- Çakışma kontrolü
- Hasta ve doktor aktiflik kontrolü

Bu tasarım size nasıl görünüyor? Hangi kısımları değiştirmek veya eklemek istersiniz? 