using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.Text;

	namespace VGMToolbox.util.ObjectPooling
	{
		/// <summary>
		/// Summary description for ByteArray.
		/// </summary>
		public class ByteArray
		{
			private byte [] byArray;
            public byte[] ByArray 
            {
                set { byArray = value; }            
                get { return byArray; }            
            }

            private int iArrayLength = 0;
            public int ArrayLength
            {
                set { iArrayLength = value; }
                get { return iArrayLength; }
            }
            
            private bool bLocked;
			
            public ByteArray( int iMaxSize )
			{
				byArray = null;
				bLocked = false;
				try
				{
					byArray = new byte [ iMaxSize ];
				}
				catch( Exception ex )
				{
					throw new Exception( "Error trying to create byte array: " + ex.Message );
				}
			}

			public byte [] GetByteArray() { return byArray; }
			private void Reset() { byArray.Initialize(); }
			
            public bool Lock()
			{
				lock( this ) 
				{
					if( bLocked )
						return false;

					bLocked = true;

					Reset();
					return true;
				}
			}
			public void Unlock()
			{
				lock( this )
				{
					bLocked = false;
				}
			}
			public bool IsLocked() { return bLocked; }
		}

		/// <summary>
		/// Summary description for PoolByteArrays.
		/// </summary>
		public class PoolByteArrays
		{
			private ArrayList byarArray;
			private int iCurrentPoolSize;
			private int iMaxByteArraySize;

			public PoolByteArrays( int iMaxConcurrentThreads, int iMaxSize )
			{
				byarArray = null;
				iCurrentPoolSize = 0;
				iMaxByteArraySize = iMaxSize;

				if( iMaxConcurrentThreads < 1 )
					throw new Exception( "Construction of PoolByteArrays failed - iMaxConcurrentThreads is less than 1" );

				try
				{
					byarArray = new ArrayList( iMaxConcurrentThreads );
					for( int iCnt = 0; iCnt < iMaxConcurrentThreads; iCnt++ )
						byarArray.Add( new ByteArray( iMaxSize ) );

					iCurrentPoolSize = iMaxConcurrentThreads;
				}
				catch( Exception ex )
				{
					throw new Exception( "Construction of PoolByteArrays failed - " + ex.Message );
				}
			}

			public ByteArray GetFreeByteArray(ref int iIndex)
			{
				try
				{
					for( int jCnt = 1; jCnt <= 2; jCnt++ )
					{
						for( int iCnt = 0; iCnt < iCurrentPoolSize; iCnt++ )
						{
							ByteArray byarInstance = ( ByteArray ) byarArray[ iCnt ];

							if( byarInstance.IsLocked() )
								continue;
							if( ! byarInstance.Lock() )
								continue;

                            iIndex = iCnt;
                            byarInstance.ByArray.Initialize();
                            byarInstance.ArrayLength = 0;
                            return byarInstance;
						}
						Thread.Sleep( 100 );
					}
				}
				catch( Exception ex )
				{
					throw new Exception( "Exception trying to locate and lock a free byte array. Details: " + ex.Message );
				}

				lock( this )
				{
					try
					{
						iIndex = byarArray.Add( new ByteArray( iMaxByteArraySize ) );
						ByteArray byarInstance = ( ByteArray ) byarArray[ iIndex ];
						byarInstance.Lock();
						iCurrentPoolSize++;                                                
                        
                        return byarInstance;
					}
					catch( Exception ex )
					{
						throw new Exception( "Exceeded projected maximum concurrent usage on this TRANSACT system: error while adding new byte array. Details: " + ex.Message );
					}
				}
			}

			public void DoneWithByteArray( int iID )
			{
				try
				{
					( ( ByteArray ) byarArray[ iID ] ).Unlock();
				}
				catch( Exception ex )
				{
					throw new Exception( "Exception trying to unlock a byte array. Details: " + ex.Message );
				}
			}

			public void DestroyPool()
			{
				for( int iCnt = 0; iCnt < iCurrentPoolSize; iCnt++ )
				{
					ByteArray byarInstance = ( ByteArray ) byarArray[ iCnt ];
					byarInstance.Unlock();
				}
				byarArray.Clear();
			}
		}
	}
