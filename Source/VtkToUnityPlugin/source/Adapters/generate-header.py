import os

adapter_utility_h_name = "vtkAdapterUtility.h"

adapter_utility_h = open(adapter_utility_h_name, "w")

adapter_utility_h.write("#pragma once\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("/* This file has been automatically generated through the Python header generation utility\n")
adapter_utility_h.write(" * \n")
adapter_utility_h.write(" * This file contains the necessary information to allow the VtkToUnity plugin to know\n")
adapter_utility_h.write(" * that the adapters exist and it can call them. As such, this should be generated every\n")
adapter_utility_h.write(" * time the plugin is built to avoid losing any adapters in the compilation.\n")
adapter_utility_h.write(" */\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("// Adapters' header files found in the folder (.h and .hpp)\n")
adapter_utility_h.write("\n")

adapter_utility_h.write("// Utility includes\n")
adapter_utility_h.write("#include <unordered_map>\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("#define NOMINMAX\n")
adapter_utility_h.write("#include <windows.h>\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("#include \"../Singleton.h\"\n")
adapter_utility_h.write("#include \"../vtkAdapter.h\"\n")
adapter_utility_h.write("\n")

classes = []

# Generating the includes of the adapters' header files

for file in os.listdir("."):
    if not file == adapter_utility_h_name and ( file.endswith(".h") or file.endswith(".hpp") ):
        adapter_utility_h.write(f"#include \"{file}\"\n")
        class_name = os.path.splitext(file)[0]
        classes.append(class_name[:1].upper() + class_name[1:])

adapter_utility_h.write("\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("// This class is used to register the adapters\n")
adapter_utility_h.write("class VtkAdapterUtility\n")
adapter_utility_h.write("{\n")

# Generating the class for the utility operations

adapter_utility_h.write("public:\n")

# begin public area

adapter_utility_h.write("\tstatic void RegisterAll()\n")
adapter_utility_h.write("\t{\n")

# begin RegisterAll

for clss in classes:
    adapter_utility_h.write(f"\t\ts_adapters[Singleton<{clss}>::Instance()->GetAdaptingObject()] = Singleton<{clss}>::Instance();\n")

# end RegisterAll

adapter_utility_h.write("\t}\n")
adapter_utility_h.write("\n")
adapter_utility_h.write("\tstatic VtkAdapter* GetAdapter(\n")
adapter_utility_h.write("\t\tLPCSTR vtkAdaptedObject)\n")
adapter_utility_h.write("\t{\n")

# begin GetAdapter

adapter_utility_h.write("\t\tauto itAdapter = s_adapters.find(vtkAdaptedObject);\n")
adapter_utility_h.write("\t\tif (itAdapter != s_adapters.end())\n")
adapter_utility_h.write("\t\t{\n")
adapter_utility_h.write("\t\t\treturn itAdapter->second;\n")
adapter_utility_h.write("\t\t}\n")
adapter_utility_h.write("\t\telse\n")
adapter_utility_h.write("\t\t{\n")
adapter_utility_h.write("\t\t\treturn NULL;\n")
adapter_utility_h.write("\t\t}\n")

# end GetAdapter

adapter_utility_h.write("\t}\n")
adapter_utility_h.write("\n")

# end public area

adapter_utility_h.write("private:\n")

# begin private area

adapter_utility_h.write("\t// Map with all the adapters registered in this folder\n")
adapter_utility_h.write("\tstatic std::unordered_map<LPCSTR, VtkAdapter*> s_adapters;\n")

# end private area

adapter_utility_h.write("};\n")