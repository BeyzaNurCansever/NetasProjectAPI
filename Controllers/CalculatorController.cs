using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using System.IO;
using System;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace DefaultNamespace;

[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CalculatorController:Controller
{
    private CalculatorDbContext _calculatorDbContext;

    public CalculatorController(CalculatorDbContext calculatorDbContext)
    {
        _calculatorDbContext = calculatorDbContext;
    }

    [HttpPost]
    // [Authorize("Manager")]  rol atayarak sınırlı erişim sağlama
    [Authorize]
    public async Task<ActionResult> GetData( CalculateModel calculateModel)
    {
        var blobStorageConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=YourAccountName;AccountKey=YourAccountKey";
        var blobStorageContainerName = "YourContainerName";
        var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);
        BlobServiceClient blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobStorageContainerName);
        
        
        int blobCount = 0;
        foreach (BlobItem blobItem in containerClient.GetBlobs())
        {
            blobCount++;
        }
           
        //txt oluşturup içine yazma
        string dosyaismi = "dosya" + blobCount;
        string dosyayolu = dosyaismi+".txt";
        string content = calculateModel.result.ToString();
        
        using (StreamWriter writer = new StreamWriter(dosyayolu))
        {
            writer.WriteLine(content);
            writer.Close();
        }
        Console.WriteLine("dsoyaya yazma tamamlandı");
        
        //database e kaydetme
        Console.WriteLine(calculateModel.result);
        Console.WriteLine(calculateModel.Id);
      
        await _calculatorDbContext.CalculateModels.AddAsync(calculateModel);
        await _calculatorDbContext.SaveChangesAsync();
        
        //blob a kaydetmek gerekli connection değişkenlerin tanımlanması
        // BlobClient oluşturun
        BlobClient blobClient = containerClient.GetBlobClient(dosyaismi+".txt");
        using FileStream fs = System.IO.File.OpenRead(dosyaismi+".txt");
        await blobClient.UploadAsync(fs,true);
        Console.WriteLine("Upload Completed!");
        
        
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public IActionResult get()
    {
        Console.WriteLine("hello");
        return Ok();
    }
    
}