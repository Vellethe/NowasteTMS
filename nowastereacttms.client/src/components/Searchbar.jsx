import React, { useState } from "react";

const SearchBar = ({ onFilterChange, disabled }) => {
  const [searchValue, setSearchValue] = useState("");

  const handleChange = (e) => {
    if (!disabled) {
    const { value } = e.target;
    setSearchValue(value); // Update the search value
    onFilterChange(value);
    }
  };

  return (
    <input
      className="text-center w-full p-1 my-1"
      type="text"
      placeholder={disabled ? "" : "Search"}
      onChange={handleChange}
      value={searchValue}
      disabled={disabled}
    />
  );
};

export default SearchBar;
