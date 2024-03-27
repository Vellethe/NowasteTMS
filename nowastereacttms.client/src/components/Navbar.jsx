import { Link } from "react-router-dom";
import NWLlogo from "../assets/Images/NWLlogo.jpg";
import React, { useState } from "react";
import { FaRegUser } from "react-icons/fa6";
import { RxHamburgerMenu } from "react-icons/rx";
import { IoMdClose } from "react-icons/io";
import "@fontsource/cabin";

const loggedInNavMenu = [
  {
    name: "Orders",
    link: "/Transport/Order/AllOrders",
  },
  {
    name: "Transport Orders",
    link: "/Transport/TransportOrder/AllTransportOrders",
  },
  {
    name: "Prices",
    link: "/Transport/TransportZonePrices",
  },
  {
    name: "Agents",
    link: "/Transport/Agents/AllAgents",
  },
  {
    name: "Customers",
    link: "/Transport/Customers/AllCustomers",
  },
  {
    name: "Supplier",
    link: "/Transport/Suppliers/AllSuppliers",
  },
  {
    name: "Services",
    link: "/Transport/TransportOrderService",
  },
];

const loggedOutNavMenu = [
  {
    name: "Home",
    link: "/",
  },
  {
    name: "About",
    link: "/About",
  },
  {
    name: "Features",
    link: "/Features",
  },
  {
    name: "Customers",
    link: "/Customers",
  },
  {
    name: "Contact",
    link: "/Contact",
  },
];

const Navbar = () => {
  const [open, setOpen] = useState(false);
  const handleMenu = () => {
    setOpen((prev) => !prev);
  };
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  const handleLogin = () => {
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    setIsLoggedIn(false);
  };

  const navMenu = isLoggedIn ? loggedInNavMenu : loggedOutNavMenu;

  return (
    <header className="flex items-center justify-between flex-wrap bg-gradient-to-l from-dark-green via-medium-green to-white w-full top-0 left-0 p-6 text-white">
      <Link to="/">
        <img className="w-[80px] rounded" src={NWLlogo} alt="NWL Logo" />
      </Link>
      <ul className="hidden md:flex md:items-center gap-4 text-2xl">
        {navMenu.map((menuItem) => (
          <Link
            to={menuItem.link}
            key={menuItem.name}
            className="hover:underline underline-offset-8"
          >
            {menuItem.name}
          </Link>
        ))}
      </ul>
      <div className="hidden md:flex items-center">
        {isLoggedIn ? (
          <>
            <Link to="/Identity/Account/Login">
              <FaRegUser className="text-2xl m-3 hover:text-3xl cursor-pointer" />
            </Link>

            <button
              type="button"
              onClick={handleLogout}
              className="text-white bg-brown hover:bg-white hover:text-medium-green duration-200 text-xl p-2 border border-solid rounded-3xl"
            >
              Log out
            </button>
          </>
        ) : (
          <button
            type="button"
            onClick={handleLogin}
            className="text-white bg-brown hover:bg-white hover:text-medium-green duration-200 text-xl p-2 border border-solid rounded-3xl"
          >
            Log in
          </button>
        )}
      </div>

      {/* ----mobile--- */}
      <div className="-mr-2 flex flex-row-reverse p-4 md:hidden">
        <div className="flex items-center ml-4">
          <button type="button" onClick={handleMenu} className="flex text-3xl ">
            {open == true ? <IoMdClose /> : <RxHamburgerMenu />}
          </button>
        </div>
        {open ? (
          <div className="md:hidden -center">
            <div className="text-center">
              {navMenu.map((menuItem) => (
                <Link
                  to={menuItem.link}
                  key={menuItem.name}
                  className="hover:underline underline-offset-8 block m-2 "
                >
                  {menuItem.name}
                </Link>
              ))}
              <Link to="/Identity/Account/Login">
                <button
                  type="button"
                  className="text-white bg-brown hover:bg-white hover:text-medium-green duration-200 text-sm w-[100px] mt-4 p-2 border border-solid rounded"
                >
                  Log out
                </button>
              </Link>
            </div>
          </div>
        ) : null}
      </div>
    </header>
  );
};

export default Navbar;
