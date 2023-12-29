import { Upload } from "./components/Upload";
import { Assemblies } from "./components/Assemblies";
import Assembly from "./components/Assembly";
import { Tests } from "./components/Tests";
import Test from "./components/Test";

const AppRoutes = [
  { 
    index: true,
    element: <Upload />
  },
  { 
    path: '/assemblies',
    element: <Assemblies />
  },
  { 
    path: '/testList',
    element: <Tests />
  },
  { 
    path: '/test/:id',
    element: <Test />
  },
  { 
    path: `/assemblies/:id`,
    element: <Assembly />
  }
];

export default AppRoutes;
