(function () {

    "use strict"; 
    angular.module("cbspay")
        .controller("CBSNetPayController", CBSNetPayController);

    function CBSNetPayController($scope, $http) { 

        $scope.formData = {}

        $scope.payTaxPayer = function (item) { 

            var config = {
                headers: { 
                    'ClientId': '4527ab2b-3651-4017-9399-358d01bb9558',
                    'ClientSecret': 'bJt+i61Vo71Y4T8nIcjxZMMMYyByQ4YlAxrE/vSEWPlDdVMYHgFdVAsLbRDypDpzjki7LE+q8wSP0PiheC8vOQ=='
                }
            }
             
            item.ReferenceAmount = item.totalAmount
            item.PaymentIdentifier = "NET_23467ED353453E"
            item.PhoneNumber = "07064548188"
            item.SettlementAmount = 70000
            item.SettlementDate = new Date()
          

            alert("Hi I am about to pay taxpayer")

            $http.post('api/cbsnetpay/makepayment', item, config)

                .success(function (data, status, headers, config) {

                    //call the function to redirect to the netpay page.
                   $scope.formData = data
                   $scope.callnetpay($scope.formData);
                    
                })
                .error(function (data, status, header, config) {
                    $scope.ResponseDetails = "Data: " + data +
                        "<hr />status: " + status +
                        "<hr />headers: " + header +
                        "<hr />config: " + config;
                });
           
        } 

        $scope.callnetpay = function (data) {
            //console.log($scope.formData)

            var data = {
                'MerchantUniqueId': '4527ab2b-3651-4017-9399-358d01bb9558',
                'HMAC': 'bJt+i61Vo71Y4T8nIcjxZMMMYyByQ4YlAxrE/vSEWPlDdVMYHgFdVAsLbRDypDpzjki7LE+q8wSP0PiheC8vOQ==',
                'Currency': 'NGN',
                'TransactionReference': 'NET_23467ED353453E',
                'Amount': '600000',
                'ReturnUrl': 'cbs.com/api'
            }

           $scope.$broadcast('gateway.redirect');

            $http.post('http://parkwaydev.cloudapp.net/netpayng/app/Home/Pay/', data)

                .success(function (data, status, headers, config) {
                     
                })
                .error(function (data, status, header, config) {
                     
                });
        }
    }


})();


 


 