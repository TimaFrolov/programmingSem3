import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

export const Upload = () => {
  const [selectedFiles, setSelectedFiles] = useState([]);
  const navigate = useNavigate();

  const handleFileChange = (event) => {
    setSelectedFiles([...event.target.files]);
  };

  const handleUpload = () => {
    const formData = new FormData();
    selectedFiles.forEach((file) => {
      formData.append("files", file);
    });

    fetch("assembly", {
      method: "POST",
      body: formData,
    })
      .then((response) => response.json())
      .then((data) => navigate(`/assemblies/${data.id}`));
  };

  return (
    <div>
      <h2>Загрузить сборку</h2>
      <input type="file" multiple onChange={handleFileChange} />
      <button onClick={handleUpload}>Загрузить</button>
    </div>
  );
};
