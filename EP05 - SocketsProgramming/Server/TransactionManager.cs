#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Shared;

namespace Server
{
    public class TransactionManager
    {

        readonly List<SubmitBasketRequest> _transactions = new List<SubmitBasketRequest>();


        public POSController? POSController { get; set; }


        public void ProcessBasket( SubmitBasketRequest r ) => _transactions.Add( r );

        public async Task PayBasket( )
        {

            if ( _transactions.Count > 0 ) {

                var basket = _transactions[0];
                _transactions.RemoveAt( 0 );

                var payBasketRequest = new BasketPaidRequest {
                    Id = Guid.NewGuid().ToString(),
                    POSTransactionNumber = basket.POSTransactionNumber,
                    POSData = basket.POSData,
                    PaymentInfo = new PaymentInfo {
                        Amount = 50.00m,
                        AuthorizationCode = "AUTH1234",
                        LastFour = "9876"
                    }
                };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await POSController.SendTo( payBasketRequest ).ConfigureAwait( false );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            }
        }


    }
}
