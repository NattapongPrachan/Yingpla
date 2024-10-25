using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AWSCloudKey", menuName = "Addressables/CloudKey")]
public class CloudKey : ScriptableObject
{
    public string m_BucketName;
    public string m_AwsAccessKey;
    public string m_AwsSecrentKey;

    public string BucketName { get { return Decrypt(m_BucketName); } }
    public string AWSAccessKey { get { return Decrypt(m_AwsAccessKey); } }
    public string AWSSecretKey { get { return Decrypt(m_AwsSecrentKey); } }

    private static string Decrypt(string encrypt)
    {
        byte[] code;
        string decrypt;
        #region decrypt
        try
        {
            code = Convert.FromBase64String(encrypt);
            decrypt = System.Text.ASCIIEncoding.ASCII.GetString(code);


        }
        catch (System.Exception)
        {
            decrypt = "";
            throw;
        }
        return decrypt;
        #endregion
    }

}
