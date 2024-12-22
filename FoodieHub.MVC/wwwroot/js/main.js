const mobileNav = document.getElementById('mobileNav');
const searchContent = document.getElementById("search-content")
document.getElementById('toggleMenu').addEventListener('click', function () {
    mobileNav.classList.toggle('show');
    searchContent.classList.remove("show-search")
});

const toggleSearch = document.getElementById("toggleSearch")
if (toggleSearch) {
    toggleSearch.addEventListener("click", () => {
        searchContent.classList.toggle("show-search")
        mobileNav.classList.remove('show');
    })
}


const navLinks = document.querySelectorAll(".nav__link");
navLinks.forEach(element => {
    element.addEventListener("click", (event) => {
        navLinks.forEach(link => {
            link.parentElement.classList.remove("active");
        });
        event.target.parentElement.classList.add("active");
    });
});


document.querySelectorAll('.decrease-quantity').forEach(button => {
    button.addEventListener('click', function () {
        const form = this.closest('.update-cart-form'); 
        const inputField = form.querySelector('.quantity-input'); //
        let quantity = parseInt(inputField.value) - 1;
        quantity = Math.max(quantity, 1); 
        inputField.value = quantity;
        form.submit();
    });
});

document.querySelectorAll('.increase-quantity').forEach(button => {
    button.addEventListener('click', function () {
        const form = this.closest('.update-cart-form'); 
        const inputField = form.querySelector('.quantity-input'); 
        const maxStock = parseInt(inputField.getAttribute('data-max-stock'));
        let quantity = parseInt(inputField.value);
        const contactModal = new bootstrap.Modal(document.getElementById('contactModalCheckOut'));
        
        if (quantity < maxStock) {
            quantity++;
            inputField.value = quantity;
            form.submit();
        } else {
            alert(`The maximum available stock is ${maxStock}.`);
            contactModal.show(); 
        }
    });
});


const couponID = document.getElementById("couponID");
const couponInput = document.getElementById("couponInput");
const message = document.getElementById("Message");
const subTotalElement = document.getElementById("subTotal");
const baseUrlElement = document.getElementById("baseApiUrl");

if (couponID && couponInput && message && subTotalElement && baseUrlElement) {
    const subTotal = parseFloat(subTotalElement.innerText);
    const baseUrl = baseUrlElement.value;

    couponInput.addEventListener("blur", () => {
        const couponCode = couponInput.value.trim();

        if (!couponCode) {
            message.innerText = "Please enter a valid coupon code.";
            return;
        }

        fetch(`${baseUrl}api/coupons/couponcode/${couponCode}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const coupon = data.data;
                    const couponDiscount = document.getElementById("CouponDiscount");
                    const totalAmountElement = document.getElementById("TotalAmount");

                    if (couponDiscount && totalAmountElement) {
                        if (parseFloat(coupon.minimumOrderAmount) > subTotal) {
                            couponInput.value = "";
                            couponID.value = "";
                            message.innerText = `The order must be at least $${coupon.minimumOrderAmount} to apply this coupon.`;
                        } else {
                            let discountOfCoupon = 0;

                            if (coupon.discountType === "Fixed") {
                                discountOfCoupon = coupon.discountValue;
                            } else {
                                discountOfCoupon = subTotal * (coupon.discountValue / 100);
                            }

                            couponDiscount.innerHTML = `
                                <span>Coupon Discount</span>
                                <span>- $${discountOfCoupon.toFixed(2)}</span>
                            `;

                            let totalAmount = subTotal - discountOfCoupon;
                            totalAmountElement.innerText = `$${totalAmount.toFixed(2)}`;

                            message.innerText = "Coupon applied successfully.";

                            couponID.value = coupon.couponID;
                        }
                    }
                } else {
                    resetCouponInfo();
                    message.innerText = data.message || "Coupon not found or invalid.";
                }
            })
            .catch(error => {
                console.error(error);
                resetCouponInfo();
                message.innerText = "Error occurred while applying the coupon. Please try again later.";
            });
    });

    // Hàm reset thông tin coupon khi có lỗi hoặc không hợp lệ
    function resetCouponInfo() {
        const couponDiscount = document.getElementById("CouponDiscount");
        const totalAmountElement = document.getElementById("TotalAmount");

        if (couponDiscount && totalAmountElement) {
            couponDiscount.innerHTML = "";
            totalAmountElement.innerText = `$${subTotal.toFixed(2)}`;
        }

        couponInput.value = "";
        couponID.value = "";
    }
} else {
    console.error("One or more required elements are missing in the DOM.");
}

                        

/*// Hàm reset thông tin coupon khi có l?i ho?c không h?p l?
function resetCouponInfo() {
    const couponDiscount = document.getElementById("CouponDiscount");
    const totalAmountElement = document.getElementById("TotalAmount");

    // Reset các thông tin v? gi?m giá và t?ng ti?n
    couponDiscount.innerHTML = "";
    totalAmountElement.innerText = `$${subTotal.toFixed(2)}`;

    couponInput.value = "";
    couponID.value = "";
}*/





document.getElementById('orderForm').addEventListener('submit', function (event) {
    var isValid = true;
    // Validate Phone Number
    var phone = document.getElementById('phone');
    var phoneError = document.getElementById('phone-error');
    var phonePattern = /^(84|0[3|5|7|8|9])([0-9]{8})$/;
    if (!phonePattern.test(phone.value)) {
        phoneError.style.display = 'block';
        isValid = false;
    } else {
        phoneError.style.display = 'none';
    }
    if (!isValid) {
        event.preventDefault();
    }
});





const selectProvince = document.getElementById("province")
const selectDistrict = document.getElementById("district");
const selectWard = document.getElementById("ward");

// Fetch Provinces using fpo.vn API
fetch('https://vn-public-apis.fpo.vn/provinces/getAll?limit=-1')
    .then(response => response.json())
    .then(data => {
        if (data && data.data && data.data.data) {
            const provinces = data.data.data;  // Assuming data.data.data holds the list of provinces
            provinces.forEach(item => {
                selectProvince.innerHTML += `<option data-id="${item.code}" value="${item.name}">${item.name}</option>`;
            });
        } else {
            console.error('Invalid province response structure:', data);
        }
    })
    .catch(error => console.error('Error fetching provinces:', error));

// Handle province change
selectProvince.addEventListener("change", () => {
    const selectedOption = selectProvince.options[selectProvince.selectedIndex];
    const provinceID = selectedOption.getAttribute("data-id");

    // Reset district and ward dropdowns
    selectDistrict.innerHTML = '<option value="">Select District</option>';
    selectWard.innerHTML = '<option value="">Select Ward</option>';

    // Fetch districts based on selected province
    fetch(`https://vn-public-apis.fpo.vn/districts/getByProvince?provinceCode=${provinceID}&limit=-1`)
        .then(response => response.json())
        .then(data => {
            if (data && data.data && data.data.data) {
                const districts = data.data.data;
                districts.forEach(item => {
                    selectDistrict.innerHTML += `<option data-id="${item.code}" value="${item.name_with_type}">${item.name_with_type}</option>`;
                });
            } else {
                console.error('Invalid district response structure:', data);
            }
        })
        .catch(error => console.error('Error fetching districts:', error));
});

// Handle district change
selectDistrict.addEventListener("change", () => {
    const selectedOption = selectDistrict.options[selectDistrict.selectedIndex];
    const districtID = selectedOption.getAttribute("data-id");

    // Reset ward dropdown
    selectWard.innerHTML = '<option value="">Select Ward</option>';

    // Fetch wards based on selected district
    fetch(`https://vn-public-apis.fpo.vn/wards/getByDistrict?districtCode=${districtID}&limit=-1`)
        .then(response => response.json())
        .then(data => {
            if (data && data.data && data.data.data) {
                const wards = data.data.data;
                wards.forEach(item => {
                    selectWard.innerHTML += `<option value="${item.name_with_type}">${item.name_with_type}</option>`;
                });
            } else {
                console.error('Invalid ward response structure:', data);
            }
        })
        .catch(error => console.error('Error fetching wards:', error));
});



const apiKey = '2bb727b2312a4ace85574ea17983ec16';


function getLocationAndFillForm() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;
            const url = `https://api.opencagedata.com/geocode/v1/json?q=${latitude},${longitude}&key=${apiKey}`;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    if (data.results.length > 0) {
                        const result = data.results[0];
                        console.log(result)
                        const duong = result.components.road;
                        const phuong = result.components.quarter;
                        const quan = result.components.suburb;
                        const city = result.components.city || result.components.town;
                        
                        document.getElementById('address').value = `${duong}`;

                     
                        updateProvinceDistrictWard(phuong, quan, city);
                    } else {
                        console.log('Không tìm th?y ??a ch? cho v? trí này.');
                    }
                })
                .catch(error => {
                    console.error('Lỗi khi gửi OpenCageData API:', error);
                });
        }, function (error) {
            console.log(error)
        });
    } else {
        console.log("Trình duyet cua ban không ho tro Geolocation.");
    }
}


document.getElementById('getLocationButton').addEventListener('click', () => {
    getLocationAndFillForm();
});


function updateProvinceDistrictWard(phuong, quan, city) {
    // Tự động chọn tỉnh
    const provinceOptions = Array.from(selectProvince.options);
    const province = provinceOptions.find(option => isMatchingLocation(city, option.value));
    if (province) {
        province.selected = true;
        selectProvince.dispatchEvent(new Event('change')); // Kích hoạt sự kiện để load huyện

        // Đợi load huyện
        const districtInterval = setInterval(() => {
            const districtOptions = Array.from(selectDistrict.options);
            const district = districtOptions.find(option => isMatchingLocation(quan, option.value));
            if (district) {
                district.selected = true;
                selectDistrict.dispatchEvent(new Event('change')); // Kích hoạt sự kiện để load xã
                clearInterval(districtInterval);
            }
        }, 1000);

        // Đợi load xã
        const wardInterval = setInterval(() => {
            const wardOptions = Array.from(selectWard.options);
            const ward = wardOptions.find(option => isMatchingLocation(phuong, option.value));
            if (ward) {
                ward.selected = true;
                clearInterval(wardInterval);
            }
        }, 1000);
    }
}


function normalizeName(str) {
    if (!str) return '';
    const noDiacritics = str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    const cleaned = noDiacritics.replace(/\b(City|Province|Ward|District|Thanh pho|Phuong|Tinh|Quan|Huyen|Xa)\b/gi, "").trim();
    return cleaned.toLowerCase();
}

function isMatchingLocation(name1, name2) {
    const normalized1 = normalizeName(name1);
    const normalized2 = normalizeName(name2);
    return normalized1 === normalized2;
}