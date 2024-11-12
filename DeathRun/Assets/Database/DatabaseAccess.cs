using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseAccess : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://testUser:testPassword@cluster0.2pzu3.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
    IMongoDatabase database;
    IMongoCollection<BsonDocument> collection;

    void Start()
    {
        database = client.GetDatabase("LoginDB");
        collection = database.GetCollection<BsonDocument>("LoginCollection");
    }

    public async void SaveUserInformToDataBase(string id, string password, string nickname)
    {
        var document = new BsonDocument() { 
            { "Id", id },
            {"Password", password },
            {"Nickname", nickname }
        };
        await collection.InsertOneAsync(document);
        //컬렉션 갱신
        collection = database.GetCollection<BsonDocument>("LoginCollection");
    }

    public bool CheckUserInformsFromDataBase(string id, string password)
    {
        List<UserInform> userInforms = new List<UserInform>();

        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Id", id);
        List<BsonDocument> results = collection.Find(filter).ToList();

        if (results.Count == 0)
        {
            Debug.Log("아이디를 찾을 수 없습니다.");
            return false;
        }

        foreach (BsonDocument result in results)
        {
            UserInform userInform = new UserInform();
            if(password == result.GetElement("Password").Value.ToString())
            {
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class UserInform
{
    public string Id { get; set; }
    public string Password { get; set; }
}