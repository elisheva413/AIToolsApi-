# ?? WebApiShop - מערכת חנות תכשיטים (REST API)

מערכת Backend מתקדמת לניהול חנות תכשיטים וירטואלית, ממומשת כ-REST API מודרני באמצעות **ASP.NET Core 9** ו-**C#**.
המערכת נבנתה תוך הקפדה על עקרונות ה-REST, ביצועים גבוהים (High Performance), סקילביליות (Scalability) והפרדה מוחלטת בין השכבות (Decoupling).

## ?? ארכיטקטורה ומבנה הפרויקט

הפרויקט בנוי ב**ארכיטקטורת 3 שכבות (N-Tier Architecture)**, המאפשרת תחזוקה קלה, קוד נקי, ובדיקות איכותיות:

1. **שכבת Application (Web API):**
   * ניהול הבקשות (Controllers) והגדרות ה-Routing.
   * מימוש Middlewares מותאמים אישית (שגיאות, מעקב).
   * הגדרת Dependency Injection (DI) בצורה מרוכזת.

2. **שכבת Services (לוגיקה עסקית):**
   * מתווכת בין ה-Controllers ל-Repositories.
   * מכילה את כל הלוגיקה העסקית של חנות התכשיטים (למשל, בדיקת תקינות מחיר הזמנה).
   * הפעולות מבוצעות בצורה **אסינכרונית** לשחרור משאבי שרת (Threads).

3. **שכבת Repositories (גישה לנתונים):**
   * גישה למסד הנתונים באמצעות Entity Framework Core.
   * עבודה בגישת **Database First**.
   * שליפות ועדכונים מבוצעים באופן אסינכרוני לשיפור ביצועים ויכולת Scale.

## ?? טכנולוגיות ודגשים עיקריים

### ? ביצועים וניתוק תלויות
* **תכנות אסינכרוני:** שימוש ב-`async/await` לכל אורך הפרויקט לשחרור ה-Thread ושיפור ה-Scalability.
* **Dependency Injection:** הזרקת תלויות בין השכבות השונות ליצירת Decoupling (ניתוק) מלא בין הממשקים למימושים.

### ?? העברת נתונים ומיפוי (DTOs)
* **DTO (Data Transfer Object):** שימוש בשכבת DTO ייעודית על מנת להסיר תלות מעגלית (Circular Dependency) ולמנוע חשיפת ישויות מסד הנתונים החוצה (ללא שימוש ב-`[JsonIgnore]`).
* **C# Records:** אובייקטי ה-DTO ממומשים כ-`record`, אידיאלי להעברת נתונים (Immutable Data).
* **AutoMapper:** שימוש בספריית AutoMapper למיפוי אוטומטי, נקי ויעיל בין ה-Entities ל-DTOs.

### ?? ניטור, לוגים וניהול שגיאות
* **NLog:** שילוב מערכת לוגים מתקדמת השומרת מידע (Information ומעלה) לקבצים ייעודיים.
* **Error Handling Middleware:** טיפול גלובלי ואחיד בשגיאות לצרכי ניטור ומניעת קריסת השרת.
* **Rating Middleware:** כל תעבורת המערכת והבקשות מתועדות ונשמרות בטבלת `Rating` ייעודית למעקב וסטטיסטיקה.
* **Configuration:** כל הגדרות המערכת (כמו מחרוזות התחברות - Connection Strings) נשמרות בנפרד מהקוד בקובץ `appsettings.json`.
* **אבטחה:** המערכת פועלת תחת פרוטוקול HTTPS.

## ?? בדיקות (Testing)
בפרויקט הושם דגש רב על איכות הקוד ואמינותו, עם כיסוי נרחב של בדיקות אוטומטיות (מעל ל-12 טסטים):
* **Unit Tests (בדיקות יחידה):** בדיקות מבודדות לשכבת ה-Services (למשל `OrderServiceUnitTests`) ולשכבת ה-Repositories. כולל בדיקות מקיפות ללוגיקת סכום הזמנה (Happy Path ו-Unhappy Path).
* **Integration Tests (בדיקות אינטגרציה):** חבילת בדיקות מלאה לכל ה-Repositories (`Category`, `Order`, `Product`, `User`) לבדיקת תקינות התקשורת מול מסד הנתונים.
* **Database Fixture:** שימוש בתשתית מתקדמת לשיתוף חיבורי Database בין הטסטים.

## ?? מבנה התיקיות ב-Solution

```text
??? WebApiShop (Application) # Controllers, Middlewares, AppSettings
??? Services                 # Business Logic, AutoMapper Profiles, DTOs (Records)
??? Repositeries             # DB Context, Entities (EF), Repository Implementations
??? Tests                    # Unit Tests, Integration Tests, Database Fixture