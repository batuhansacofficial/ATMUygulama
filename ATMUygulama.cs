using System;
using System.IO;
using System.Collections.Generic;

class BankingApp
{
    private static string usersFile = "users.txt";
    private static string logFile = "transaction_log.txt";
    private static Dictionary<string, double> accounts = new Dictionary<string, double>();

    static void Main()
    {
        LoadUsers();
        Console.WriteLine("ATM uygulamasına hoşgeldiniz!");
        Console.Write("Kullanıcı adı: ");
        string username = Console.ReadLine();
        Console.Write("Şifre: ");
        string password = Console.ReadLine();

        if (AuthenticateUser(username, password))
        {
            Console.WriteLine("Giriş başarılı!\n");
            ShowMenu(username);
        }
        else
        {
            Console.WriteLine("Kullanıcı bulunamadı.");
            LogTransaction($"{username} kullanıcısı için geçersiz giriş bilgileri");
        }
    }

    static void ShowMenu(string username)
    {
        while (true)
        {
            Console.WriteLine("\nBanka İşlemleri:");
            Console.WriteLine("1. Para Yatırma");
            Console.WriteLine("2. Para Çekme");
            Console.WriteLine("3. Para Aktarma");
            Console.WriteLine("4. Bakiye Görüntüleme");
            Console.WriteLine("5. Çıkış");
            Console.Write("Yapmak istediğiniz işlemin numarası: ");

            switch (Console.ReadLine())
            {
                case "1": Deposit(username); break;
                case "2": Withdraw(username); break;
                case "3": Transfer(username); break;
                case "4": CheckBalance(username); break;
                case "5": return;
                default: Console.WriteLine("Geçersiz işlem numarası."); break;
            }
        }
    }

    static void Deposit(string username)
    {
        Console.Write("Yatırmak istediğiniz tutar: ");
        double amount = Convert.ToDouble(Console.ReadLine());
        accounts[username] += amount;
        LogTransaction($"{username} {amount:C}TL tutarında para yatırdı.");
    }

    static void Withdraw(string username)
    {
        Console.Write("Çekmek istediğiniz tutar: ");
        double amount = Convert.ToDouble(Console.ReadLine());
        if (accounts[username] >= amount)
        {
            accounts[username] -= amount;
            LogTransaction($"{username} {amount:C}TL tutarında para çekti.");
        }
        else
        {
            Console.WriteLine("Yetersiz bakiye.");
        }
    }

    static void Transfer(string username)
    {
        Console.Write("Para aktaracağınız kullanıcı adı: ");
        string recipient = Console.ReadLine();
        Console.Write("Aktaracağınız tutar: ");
        double amount = Convert.ToDouble(Console.ReadLine());

        if (accounts.ContainsKey(recipient) && accounts[username] >= amount)
        {
            accounts[username] -= amount;
            accounts[recipient] += amount;
            LogTransaction($"{username}, {recipient} kullanıcısına {amount:C}TL tutarında para gönderdi.");
        }
        else
        {
            Console.WriteLine("Kullanıcı bulunamadı veya yetersiz bakiye.");
        }
    }

    static void CheckBalance(string username)
    {
        Console.WriteLine($"Bakiyeniz: {accounts[username]:C}");
        LogTransaction($"{username} bakiyesini görüntüledi.");
    }

    static bool AuthenticateUser(string username, string password)
    {
        foreach (var line in File.ReadAllLines(usersFile))
        {
            var parts = line.Split(',');
            if (parts.Length == 3 && parts[0] == username && parts[1] == password)
            {
                accounts[username] = double.Parse(parts[2]);
                return true;
            }
        }
        return false;
    }

    static void LogTransaction(string message)
    {
        string logMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {message}";
        File.AppendAllText(logFile, logMessage + "\n");
    }

    static void LoadUsers()
    {
        if (!File.Exists(usersFile))
        {
            File.WriteAllText(usersFile, "user1,password1,1000\nuser2,password2,1500");
        }
    }
}